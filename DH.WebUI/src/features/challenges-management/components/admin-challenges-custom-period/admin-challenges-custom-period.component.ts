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

interface ICustomPeriodForm {
  rewards: FormArray;
  challenges: FormArray;
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
  constructor(
    public override readonly toastService: ToastService,
    private readonly rewardsService: RewardsService,
    private readonly gamesService: GamesService,
    private readonly challengesService: ChallengesService,
    private readonly tenantSettingsService: TenantSettingsService,
    private readonly dialog: MatDialog,
    private readonly fb: FormBuilder
  ) {
    super(toastService);

    this.fetchRewardList();
    this.fetchGameList();
    this.fetchSettings();
    this.fetchCustomPeriod();
    this.form = this.initFormGroup();
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

        // this.form.markAsPristine();
      },
      error: (error) => {
        this.toastService.error({
          message: 'Failed to load custom period',
          type: ToastType.Error,
        });
      },
    });
  }

  public canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
    if (!this.tenantSettings?.isCustomPeriodOn) return true;

    if (this.isUnsavedChanges || this.form.dirty) {
      console.log('hasUnsavedChanges');

      return new Promise<boolean>((resolve) => {
        const dialogRef = this.dialog.open(UnsavedChangesConfirmationDialog);
        dialogRef.afterClosed().subscribe({
          next: (confirmed: boolean) => resolve(confirmed),
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

  public onSave() {
    if (this.form.valid) {
      this.errors = [];
      const customPeriod = this.parseFormValueToCustomPeriod();
      this.challengesService.saveCustomPeriod(customPeriod).subscribe({
        next: () => {
          this.toastService.success({
            message: AppToastMessage.ChangesSaved,
            type: ToastType.Success,
          });

          if (
            customPeriod.challenges.length !== 0 &&
            customPeriod.rewards.length !== 0
          ) {
            this.isCustomPeriodInitialized = true;
            this.isUnsavedChanges = false;
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

          console.log('All error messages:', this.errors);
          this.toastService.error({
            message: AppToastMessage.FailedToSaveChanges,
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
        message: `You have reached the maximum allowed rewards (${maxRewards}).
        To add more, adjust the global settings.`,
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
    throw new Error('Method not implemented.');
  }

  public fetchSettings(): void {
    this.tenantSettingsService.get().subscribe({
      next: (res) => {
        this.tenantSettings = res;
      },
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
      error: (error) => {
        console.log(error);
      },
    });
  }

  private fetchGameList(): void {
    this.gamesService.getDropdownList().subscribe({
      next: (gameList) => {
        this.gameList = gameList ?? [];
      },
      error: (error) => {
        console.log(error);
      },
    });
  }
}
