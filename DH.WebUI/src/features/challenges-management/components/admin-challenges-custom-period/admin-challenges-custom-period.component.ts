import { SupportLanguages } from './../../../../entities/common/models/support-languages.enum';
import { LanguageService } from './../../../../shared/services/language.service';
import { CustomPeriodLeaveConfirmationDialog } from './../../dialogs/custom-period-leave-confirmation/custom-period-leave-confirmation.component';
import { ITenantSettings } from './../../../../entities/common/models/tenant-settings.model';
import { Component } from '@angular/core';
import { IGameDropdownResult } from '../../../../entities/games/models/game-dropdown.model';
import { GamesService } from '../../../../entities/games/api/games.service';
import { ToastService } from '../../../../shared/services/toast.service';
import { Form } from '../../../../shared/components/form/form.component';
import { Formify } from '../../../../shared/models/form.model';
import {
  AbstractControl,
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { RewardsService } from '../../../../entities/rewards/api/rewards.service';
import { IRewardDropdownResult } from '../../../../entities/rewards/models/reward-dropdown.model';
import { TenantSettingsService } from '../../../../entities/common/api/tenant-settings.service';
import { CanComponentDeactivate } from '../../../../shared/guards/can-component-deactivate.interface';
import { Observable } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { ToastType } from '../../../../shared/models/toast.model';
import { ChallengesService } from '../../../../entities/challenges/api/challenges.service';
import {
  ICustomPeriod,
  ICustomPeriodChallenge,
  ICustomPeriodReward,
} from '../../../../entities/challenges/models/custom-period.model';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { UnsavedChangesConfirmationDialog } from '../../../../shared/dialogs/unsaved-changes-confirmation/unsaved-changes-confirmation.dialog';
import { TranslateService } from '@ngx-translate/core';

interface ICustomPeriodForm {
  rewards: FormArray;
  challenges: FormArray;
}

enum PeriodDataAction {
  Save = 'Save',
  Leave = 'Leave',
}

function customPeriodValidator(): ValidatorFn {
  return (group: AbstractControl): ValidationErrors | null => {
    const rewards = group.get('rewards') as FormArray;
    const challenges = group.get('challenges') as FormArray;

    const hasRewards = rewards && rewards.length > 0;
    const hasChallenges = challenges && challenges.length > 0;

    // ✅ VALID if:
    // - both are filled
    // - or both are empty (intentional clearing)
    const bothEmpty = !hasRewards && !hasChallenges;
    const bothFilled = hasRewards && hasChallenges;

    return bothFilled || bothEmpty ? null : { customPeriodInvalid: true };
  };
}
@Component({
  selector: 'app-admin-challenges-custom-period',
  templateUrl: 'admin-challenges-custom-period.component.html',
  styleUrl: 'admin-challenges-custom-period.component.scss',
})
export class AdminChallengesCustomPeriodComponent
  extends Form
  implements CanComponentDeactivate
{
  override form: Formify<ICustomPeriodForm>;
  public errors: string[] = [];
  public rewardList: IRewardDropdownResult[] = [];
  public gameList: IGameDropdownResult[] = [];
  public tenantSettings: ITenantSettings | null = null;
  public isCustomPeriodInitialized = false;
  public isUnsavedChanges = false;
  public SupportLanguages = SupportLanguages;
  private hasDeletedDataChangedButUnsaved: boolean = false;

  constructor(
    public override readonly toastService: ToastService,
    private readonly rewardsService: RewardsService,
    private readonly gamesService: GamesService,
    private readonly challengesService: ChallengesService,
    private readonly tenantSettingsService: TenantSettingsService,
    private readonly dialog: MatDialog,
    private readonly fb: FormBuilder,
    public override translateService: TranslateService,
    private readonly languageService: LanguageService
  ) {
    super(toastService, translateService);

    this.fetchRewardList();
    this.fetchGameList();
    this.fetchSettings();
    this.fetchCustomPeriod();
    this.form = this.initFormGroup();
  }

  public get getCurrentLanguage(): SupportLanguages {
    return this.languageService.getCurrentLanguage();
  }

  public get getCurrentResetDayForRewards(): string {
    return this.translateService.instant(
      `week_days_names.${this.tenantSettings?.resetDayForRewards.toString()}`
    );
  }

  private fetchCustomPeriod(): void {
    this.challengesService.getCustomPeriod().subscribe({
      next: (customPeriod) => {
        // Clear existing controls
        this.rewardArray.clear();
        this.challengeArray.clear();

        if (
          customPeriod.challenges.length !== 0 &&
          customPeriod.rewards.length !== 0
        )
          this.isCustomPeriodInitialized = true;

        // Populate rewards
        customPeriod.rewards.forEach((reward) => {
          this.rewardArray.push(
            this.fb.group({
              id: [reward.id],
              selectedReward: [reward.selectedReward],
              requiredPoints: [reward.requiredPoints],
            })
          );
        });

        // Populate challenges
        customPeriod.challenges.forEach((challenge) => {
          this.challengeArray.push(
            this.fb.group({
              id: [challenge.id],
              selectedGame: [challenge.selectedGame],
              attempts: [challenge.attempts],
              points: [challenge.points],
            })
          );
        });

        this.initDeletedGames();
        this.initDeletedRewards();
      },
      error: (error) => {
        this.toastService.error({
          message: this.translateService.instant(
            'custom_period.failed_to_load_period'
          ),
          type: ToastType.Error,
        });
      },
    });
  }

  public canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
    if (!this.tenantSettings?.isCustomPeriodOn) return true;

    if (!this.checkValidityOfPeriodData(PeriodDataAction.Leave)) return false;

    if (this.isUnsavedChanges || this.form.dirty) {
      return new Promise<boolean>((resolve) => {
        const dialogRef = this.dialog.open(UnsavedChangesConfirmationDialog);
        dialogRef.afterClosed().subscribe({
          next: (confirmed: boolean) => {
            if (this.hasDeletedDataChangedButUnsaved) {
              this.toastService.error({
                message: this.translateService.instant(
                  'custom_period.has_deleted_data_changed_but_unsaved'
                ),
                type: ToastType.Error,
              });
              return resolve(false);
            }

            resolve(confirmed);
          },
          error: () => resolve(false),
        });
      });
    }

    if (!this.isCustomPeriodInitialized) {
      return new Promise<boolean>((resolve) => {
        const dialogRef = this.dialog.open(CustomPeriodLeaveConfirmationDialog);

        dialogRef.afterClosed().subscribe({
          next: (confirmed: boolean) => {
            if (confirmed && this.tenantSettings != null) {
              this.tenantSettingsService
                .update({
                  ...this.tenantSettings,
                  isCustomPeriodOn: false,
                })
                .subscribe({
                  next: () => resolve(true),
                  error: () => resolve(false),
                });
            } else {
              resolve(false);
            }
          },
          error: () => resolve(false),
        });
      });
    }

    return true;
  }

  get rewardArray(): FormArray {
    return this.form.controls['rewards'] as unknown as FormArray;
  }

  get challengeArray(): FormArray {
    return this.form.controls['challenges'] as unknown as FormArray;
  }

  public getFormGroup(formGroup: AbstractControl<any, any>): FormGroup {
    return formGroup as FormGroup;
  }
  public createReward(): FormGroup {
    return this.fb.group({
      id: new FormControl<number | null>(null),
      selectedReward: new FormControl<number | null>(null, [
        Validators.required,
      ]),
      requiredPoints: new FormControl<number | null>(null, [
        Validators.required,
      ]),
    });
  }
  public createChallenge(): FormGroup {
    return this.fb.group({
      id: new FormControl<number | null>(null),
      selectedGame: new FormControl<number | null>(null, [Validators.required]),
      attempts: new FormControl<number | null>(null, [Validators.required]),
      points: new FormControl<number | null>(null, [Validators.required]),
    });
  }

  private checkValidityOfPeriodData(action: PeriodDataAction): boolean {
    const hasGames = this.hasDeletedGames();
    const hasRewards = this.hasDeletedRewards();
    if (hasGames || hasRewards) {
      this.hasDeletedDataChangedButUnsaved = true;
    }
    if (hasGames && hasRewards) {
      this.toastService.error({
        message:
          action === PeriodDataAction.Save
            ? this.translateService.instant(
                'custom_period.validity_period_on_save.challenges_rewards_deleted'
              )
            : this.translateService.instant(
                'custom_period.validity_period_on_leave.challenges_rewards_deleted'
              ),
        type: ToastType.Error,
      });
      return false;
    }

    if (hasRewards) {
      this.toastService.error({
        message:
          action === PeriodDataAction.Save
            ? this.translateService.instant(
                'custom_period.validity_period_on_save.rewards_deleted'
              )
            : this.translateService.instant(
                'custom_period.validity_period_on_leave.rewards_deleted'
              ),
        type: ToastType.Error,
      });

      return false;
    }

    if (hasGames) {
      this.toastService.error({
        message:
          action === PeriodDataAction.Save
            ? this.translateService.instant(
                'custom_period.validity_period_on_save.games_deleted'
              )
            : this.translateService.instant(
                'custom_period.validity_period_on_leave.games_deleted'
              ),
        type: ToastType.Error,
      });
      return false;
    }

    return true;
  }

  public onSave() {
    if (!this.checkValidityOfPeriodData(PeriodDataAction.Save)) return;

    if (this.form.valid) {
      this.errors = [];
      const customPeriod = this.parseFormValueToCustomPeriod();
      this.challengesService.saveCustomPeriod(customPeriod).subscribe({
        next: () => {
          this.toastService.success({
            message: this.translateService.instant(
              AppToastMessage.ChangesSaved
            ),
            type: ToastType.Success,
          });

          if (
            customPeriod.challenges.length !== 0 &&
            customPeriod.rewards.length !== 0
          ) {
            this.isCustomPeriodInitialized = true;
            this.isUnsavedChanges = false;
            this.hasDeletedDataChangedButUnsaved = false;
            this.form.markAsPristine();
          } else if (
            customPeriod.challenges.length === 0 &&
            customPeriod.rewards.length === 0
          ) {
            this.isCustomPeriodInitialized = false;
          }
        },
        error: (errorResponse) => {
          const errors = errorResponse?.error?.errors;
          if (errors) {
            for (const key in errors) {
              if (Array.isArray(errors[key])) {
                this.errors.push(...errors[key]);
              }
            }
          }

          this.toastService.error({
            message: this.translateService.instant(
              AppToastMessage.FailedToSaveChanges
            ),
            type: ToastType.Error,
          });
        },
      });
    }
  }

  public addReward(): void {
    const maxRewards = this.tenantSettings?.challengeRewardsCountForPeriod ?? 0;
    const currentCount = this.rewardArray.length;

    if (currentCount >= maxRewards) {
      this.toastService.error({
        message: this.translateService.instant(
          'custom_period.max_rewards_reached',
          { maxRewards: maxRewards }
        ),
        type: ToastType.Error,
      });
      return;
    }
    this.isUnsavedChanges = true;
    this.rewardArray.push(this.createReward());
  }

  public addChallenge(): void {
    this.isUnsavedChanges = true;

    this.challengeArray.push(this.createChallenge());
  }

  public removeReward(index: number): void {
    this.isUnsavedChanges = true;
    this.rewardArray.removeAt(index);
  }

  public removeChallenge(index: number): void {
    this.isUnsavedChanges = true;
    this.challengeArray.removeAt(index);
  }

  protected override getControlDisplayName(controlName: string): string {
    return controlName;
  }

  public fetchSettings(): void {
    this.tenantSettingsService.get().subscribe({
      next: (res) => {
        this.tenantSettings = res;
      },
    });
  }
  get selectableGames(): IGameDropdownResult[] {
    return this.gameList.filter((g) => !g.isDeleted);
  }
  private initDeletedGames(): void {
    const challengesArray = this.form.get('challenges') as unknown as FormArray;

    if (challengesArray && challengesArray.controls.length > 0) {
      challengesArray.controls.forEach((ctrl) => {
        const selectedId = ctrl.get('selectedGame')?.value;

        if (selectedId && !this.gameList.some((g) => g.id === selectedId)) {
          this.gameList = [
            ...this.gameList,
            {
              id: selectedId,
              name: this.translateService.instant(
                'custom_period.deleted_placeholder'
              ),
              isDeleted: true,
            },
          ];
        }
      });
    }
  }

  private initDeletedRewards(): void {
    const rewardsArray = this.form.get('rewards') as unknown as FormArray;

    if (rewardsArray && rewardsArray.controls.length > 0) {
      rewardsArray.controls.forEach((ctrl) => {
        const selectedId = ctrl.get('selectedReward')?.value;

        if (selectedId && !this.rewardList.some((g) => g.id === selectedId)) {
          this.rewardList = [
            ...this.rewardList,
            {
              id: selectedId,
              name_EN: this.translateService.instant(
                'custom_period.deleted_placeholder'
              ),
              name_BG: this.translateService.instant(
                'custom_period.deleted_placeholder'
              ),
              isDeleted: true,
            },
          ];
        }
      });
    }
  }

  private hasDeletedGames(): boolean {
    const challengesArray = this.form.get('challenges') as unknown as FormArray;
    if (!challengesArray) return false;

    return challengesArray.controls.some((ctrl) => {
      const selectedId = ctrl.get('selectedGame')?.value;
      if (!selectedId) return false;

      const game = this.gameList.find((g) => g.id === selectedId);
      return !game || game.isDeleted === true;
    });
  }

  private hasDeletedRewards(): boolean {
    const rewardsArray = this.form.get('rewards') as unknown as FormArray;
    if (!rewardsArray) return false;

    return rewardsArray.controls.some((ctrl) => {
      const selectedId = ctrl.get('selectedReward')?.value;
      if (!selectedId) return false;

      const reward = this.rewardList.find((g) => g.id === selectedId);
      return !reward || reward.isDeleted === true;
    });
  }

  private initFormGroup(): FormGroup {
    return this.fb.group(
      {
        rewards: this.fb.array([]),
        challenges: this.fb.array([]),
      },
      { validators: customPeriodValidator() }
    );
  }

  private parseFormValueToCustomPeriod(): ICustomPeriod {
    const rewards: ICustomPeriodReward[] = this.rewardArray.controls.map(
      (control) => {
        const group = control as FormGroup;
        return {
          id: group.controls['id'].value ?? null,
          selectedReward: Number(group.controls['selectedReward'].value ?? 0),
          requiredPoints: Number(group.controls['requiredPoints'].value ?? 0),
        };
      }
    );

    const challenges: ICustomPeriodChallenge[] =
      this.challengeArray.controls.map((control) => {
        const group = control as FormGroup;
        return {
          id: group.controls['id'].value ?? null,
          selectedGame: Number(group.controls['selectedGame'].value ?? 0),
          attempts: Number(group.controls['attempts'].value ?? 0),
          points: Number(group.controls['points'].value ?? 0),
        };
      });

    return { rewards, challenges };
  }

  private fetchRewardList(): void {
    this.rewardsService.getDropdownList().subscribe({
      next: (rewardList) => {
        this.rewardList = rewardList ?? [];
      },
      error: () => {
        this.toastService.error({
          message: this.translateService.instant(
            AppToastMessage.SomethingWrong
          ),
          type: ToastType.Error,
        });
      },
    });
  }

  private fetchGameList(): void {
    this.gamesService.getDropdownList().subscribe({
      next: (gameList) => {
        this.gameList = gameList ?? [];
      },
      error: () => {
        this.toastService.error({
          message: this.translateService.instant(
            AppToastMessage.SomethingWrong
          ),
          type: ToastType.Error,
        });
      },
    });
  }
}
