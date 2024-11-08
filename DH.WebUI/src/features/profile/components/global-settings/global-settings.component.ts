import { Component, OnInit } from '@angular/core';
import { Form } from '../../../../shared/components/form/form.component';
import {
  FormBuilder,
  FormGroup,
  FormControl,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { NAV_ITEM_LABELS } from '../../../../shared/models/nav-items-labels.const';
import { MenuTabsService } from '../../../../shared/services/menu-tabs.service';
import { ToastService } from '../../../../shared/services/toast.service';
import { WeekDay } from '@angular/common';
import { Formify } from '../../../../shared/models/form.model';
import { TimePeriodType } from '../../../../entities/common/enum/time-period-type.enum';
import { TenantSettingsService } from '../../../../entities/common/api/tenant-settings.service';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../../shared/models/toast.model';

interface ITenantSettingsForm {
  averageMaxCapacity: number;
  challengeRewardsCountForPeriod: number;
  periodOfRewardReset: string;
  resetDayForRewards: string;
  challengeInitiationDelayHours: number;
}

interface IDropdown {
  id: number;
  name: string;
}

@Component({
  selector: 'app-global-settings',
  templateUrl: 'global-settings.component.html',
  styleUrl: 'global-settings.component.scss',
})
export class GlobalSettingsComponent extends Form implements OnInit {
  override form: Formify<ITenantSettingsForm>;
  public isMenuVisible: boolean = false;

  public weekDaysValues: IDropdown[] = [];
  public periodTimeValues: IDropdown[] = [];

  public delayHours: IDropdown[] = [
    { id: 2, name: '2' },
    { id: 3, name: '3' },
    { id: 4, name: '4' },
    { id: 5, name: '5' },
    { id: 6, name: '6' },
    { id: 7, name: '7' },
    { id: 8, name: '8' },
    { id: 9, name: '9' },
    { id: 10, name: '10' },
    { id: 11, name: '11' },
    { id: 12, name: '12' },
  ];
  public tenantSettingsId: number | null = null;
  constructor(
    private readonly fb: FormBuilder,
    public override readonly toastService: ToastService,
    private readonly menuTabsService: MenuTabsService,
    private readonly router: Router,
    private readonly tenantSettingsService: TenantSettingsService
  ) {
    super(toastService);
    this.form = this.initFormGroup();
    this.form.valueChanges.subscribe(() => {
      if (this.getServerErrorMessage) {
        this.clearServerErrorMessage();
      }
    });
    this.menuTabsService.setActive(NAV_ITEM_LABELS.GAMES);

    this.weekDaysValues = Object.entries(WeekDay)
      .filter(([key, value]) => typeof value === 'number')
      .map(([key, value]) => ({ id: value as number, name: key }));

    this.periodTimeValues = Object.entries(TimePeriodType)
      .filter(([key, value]) => typeof value === 'number')
      .map(([key, value]) => ({ id: value as number, name: key }));
  }
  public ngOnInit(): void {
    this.tenantSettingsService.get().subscribe({
      next: (res) => {
        this.tenantSettingsId = res.id ?? null;
        this.form.patchValue({
          averageMaxCapacity: res.averageMaxCapacity,
          challengeRewardsCountForPeriod: res.challengeRewardsCountForPeriod,
          periodOfRewardReset: res.periodOfRewardReset.toString(),
          resetDayForRewards: res.resetDayForRewards.toString(),
          challengeInitiationDelayHours: res.challengeInitiationDelayHours,
        });
      },
    });
  }

  public showMenu(): void {
    this.isMenuVisible = !this.isMenuVisible;
  }

  private clearServerErrorMessage() {
    this.getServerErrorMessage = null;
  }

  public onSave(): void {
    if (this.form.valid) {
      this.tenantSettingsService
        .update({
          id: this.tenantSettingsId,
          averageMaxCapacity: this.form.controls.averageMaxCapacity.value,
          challengeRewardsCountForPeriod:
            this.form.controls.challengeRewardsCountForPeriod.value,
          periodOfRewardReset: this.form.controls.periodOfRewardReset
            .value as unknown as TimePeriodType,
          resetDayForRewards: this.form.controls.resetDayForRewards
            .value as unknown as WeekDay,
          challengeInitiationDelayHours:
            this.form.controls.challengeInitiationDelayHours.value,
        })
        .subscribe({
          next: () => {
            this.toastService.success({
              message: AppToastMessage.ChangesSaved,
              type: ToastType.Success,
            });
          },
          error: (error) => {
            this.handleServerErrors(error);
            this.toastService.error({
              message: AppToastMessage.FailedToSaveChanges,
              type: ToastType.Error,
            });
          },
        });
    }
  }

  public backNavigateBtn() {
    this.router.navigateByUrl('profile');
  }

  protected override getControlDisplayName(controlName: string): string {
    throw new Error('Method not implemented.');
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      id: new FormControl<number | null>(null),
      averageMaxCapacity: new FormControl<number | null>(null, [
        Validators.required,
      ]),
      challengeRewardsCountForPeriod: new FormControl<number | null>(null, [
        Validators.required,
      ]),
      periodOfRewardReset: new FormControl<string | null>(
        'Weekly',
        Validators.required
      ),
      resetDayForRewards: new FormControl<string | null>(
        'Sunday',
        Validators.required
      ),
      challengeInitiationDelayHours: new FormControl<string | null>('2', [
        Validators.required,
      ]),
    });
  }
}
