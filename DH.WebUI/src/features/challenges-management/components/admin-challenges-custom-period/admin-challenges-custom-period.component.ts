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

interface ICustomPeriodForm {
  rewards: FormArray;
  challenges: FormArray;
}
function customPeriodValidator(): ValidatorFn {
  return (group: AbstractControl): ValidationErrors | null => {
    const rewards = group.get('rewards') as FormArray;
    const challenges = group.get('challenges') as FormArray;

    const hasRewards = rewards?.length > 0;
    const hasChallenges = challenges?.length > 0;

    return hasRewards && hasChallenges ? null : { customPeriodInvalid: true };
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

  public rewardList: IRewardDropdownResult[] = [];
  public gameList: IGameDropdownResult[] = [];
  public tenantSettings: ITenantSettings | null = null;
  public isSaved = false;

  constructor(
    public override readonly toastService: ToastService,
    private readonly rewardsService: RewardsService,
    private readonly gamesService: GamesService,
    private readonly tenantSettingsService: TenantSettingsService,
    private readonly dialog: MatDialog,
    private readonly fb: FormBuilder
  ) {
    super(toastService);

    this.fetchRewardList();
    this.fetchGameList();
    this.fetchSettings();
    this.form = this.initFormGroup();
  }

  public canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
    if (!this.tenantSettings?.isCustomPeriodOn) {
      return true;
    }

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
      selectedGame: new FormControl<number | null>(null, [Validators.required]),
      attempts: new FormControl<number | null>(null, [Validators.required]),
      points: new FormControl<number | null>(null, [Validators.required]),
    });
  }

  public onSave() {
    console.log('form', this.form.value);
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
    this.rewardArray.push(this.createReward());
  }

  public addChallenge(): void {
    this.challengeArray.push(this.createChallenge());
  }

  public removeReward(index: number): void {
    this.rewardArray.removeAt(index);
  }

  public removeChallenge(index: number): void {
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
