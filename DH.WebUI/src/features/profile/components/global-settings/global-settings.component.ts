import { Component, OnDestroy, OnInit } from '@angular/core';
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
import { IDropdown } from '../../../../shared/models/dropdown.model';
import { ToggleState } from '../../../../entities/common/enum/toggle-state.enum';
import { FULL_ROUTE } from '../../../../shared/configs/route.config';
import { SupportLanguages } from '../../../../entities/common/models/support-languages.enum';
import { TenantUserSettingsService } from '../../../../entities/common/api/tenant-user-settings.service';
import { combineLatest } from 'rxjs';
import { IUserSettings } from '../../../../entities/common/models/user-settings.model';
import { LanguageService } from '../../../../shared/services/language.service';
import { TranslateService } from '@ngx-translate/core';

interface ITenantSettingsForm {
  averageMaxCapacity: number;
  language: string;
  challengeRewardsCountForPeriod: number;
  periodOfRewardReset: string;
  resetDayForRewards: string;
  challengeInitiationDelayHours: number;
  reservationHours: string[];
  bonusTimeAfterReservationExpiration: number;
  phoneNumber: string;
  clubName: string;
  isCustomPeriodOn: number;
}

@Component({
  selector: 'app-global-settings',
  templateUrl: 'global-settings.component.html',
  styleUrl: 'global-settings.component.scss',
})
export class GlobalSettingsComponent extends Form implements OnInit, OnDestroy {
  override form: Formify<ITenantSettingsForm>;

  public userSettings: IUserSettings | null = null;
  public weekDaysValues: IDropdown[] = [];
  public periodTimeValues: IDropdown[] = [];
  public toggleStateValues: IDropdown[] = [];
  public reservationHours: IDropdown[] = [];
  public languagesValues: IDropdown[] = [];
  public oldCustomPeriodValue: ToggleState | null = null;

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
    private readonly tenantSettingsService: TenantSettingsService,
    private readonly tenantUserSettingsService: TenantUserSettingsService,
    private readonly languageService: LanguageService,
    public override translateService: TranslateService
  ) {
    super(toastService, translateService);
    this.form = this.initFormGroup();
    this.form.valueChanges.subscribe(() => {
      if (this.getServerErrorMessage) {
        this.clearServerErrorMessage();
      }
    });
    this.menuTabsService.setActive(NAV_ITEM_LABELS.PROFILE);

    this.weekDaysValues = Object.entries(WeekDay)
      .filter(([key, value]) => typeof value === 'number')
      .map(([key, value]) => ({ id: value as number, name: key }));

    this.periodTimeValues = Object.entries(TimePeriodType)
      .filter(([key, value]) => typeof value === 'number')
      .map(([key, value]) => ({ id: value as number, name: key }));

    this.toggleStateValues = Object.entries(ToggleState)
      .filter(([key, value]) => typeof value === 'number')
      .map(([key, value]) => ({ id: value as number, name: key }));

    this.languagesValues = Object.keys(SupportLanguages)
      .filter((key) => isNaN(Number(key)))
      .map((name) => ({
        id: SupportLanguages[name as keyof typeof SupportLanguages],
        name,
      })) as unknown as IDropdown[];

    let id = 1;
    for (let hour = 8; hour < 22; hour++) {
      for (let minute = 0; minute < 60; minute += 5) {
        const time = `${hour.toString().padStart(2, '0')}:${minute
          .toString()
          .padStart(2, '0')}`;
        this.reservationHours.push({ id: id, name: time });
        id++;
      }
    }
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public ngOnInit(): void {
    this.fetchSettings();
  }

  public fetchSettings(): void {
    combineLatest([
      this.tenantSettingsService.get(),
      this.tenantUserSettingsService.get(),
    ]).subscribe({
      next: ([tenantSettings, userSettings]) => {
        this.tenantSettingsId = tenantSettings.id ?? null;

        this.oldCustomPeriodValue = tenantSettings.isCustomPeriodOn
          ? ToggleState.On
          : ToggleState.Off;

        this.form.patchValue({
          averageMaxCapacity: tenantSettings.averageMaxCapacity,
          challengeRewardsCountForPeriod:
            tenantSettings.challengeRewardsCountForPeriod,
          periodOfRewardReset: tenantSettings.periodOfRewardReset.toString(),
          resetDayForRewards: tenantSettings.resetDayForRewards.toString(),
          language: userSettings.language.toString(),
          challengeInitiationDelayHours:
            tenantSettings.challengeInitiationDelayHours,
          bonusTimeAfterReservationExpiration:
            tenantSettings.bonusTimeAfterReservationExpiration,
          phoneNumber: tenantSettings.phoneNumber,
          clubName: tenantSettings.clubName,
          isCustomPeriodOn: this.oldCustomPeriodValue,
        });

        if (tenantSettings.reservationHours.length !== 0) {
          this.form.patchValue({
            reservationHours: tenantSettings.reservationHours,
          });
        }

        this.userSettings = userSettings;

        this.languageService.setLanguage(userSettings.language);
      },
    });
  }

  private clearServerErrorMessage() {
    this.getServerErrorMessage = null;
  }

  public onSave(): void {
    if (this.form.valid && this.userSettings) {
      const newLanguage = this.form.controls.language
        .value as unknown as SupportLanguages;
      const updatedSettings = {
        ...this.userSettings,
        language: newLanguage,
      };
      combineLatest([
        this.tenantSettingsService.update({
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
          reservationHours: this.form.controls.reservationHours.value,
          bonusTimeAfterReservationExpiration:
            this.form.controls.bonusTimeAfterReservationExpiration.value,
          phoneNumber: this.form.controls.phoneNumber.value,
          clubName: this.form.controls.clubName.value,
          isCustomPeriodOn:
            this.form.controls.isCustomPeriodOn.value === ToggleState.On,
        }),
        this.tenantUserSettingsService.update(updatedSettings),
      ]).subscribe({
        next: () => {
          this.toastService.success({
            message: AppToastMessage.ChangesSaved,
            type: ToastType.Success,
          });

          if (this.userSettings?.language != newLanguage) {
            console.log('newLanguage', newLanguage);

            this.languageService.setLanguage(newLanguage);
          }

          const newCustomPeriodValue =
            this.form.controls.isCustomPeriodOn.value;
          if (
            this.oldCustomPeriodValue != newCustomPeriodValue &&
            newCustomPeriodValue === ToggleState.On
          ) {
            this.router.navigateByUrl(
              FULL_ROUTE.CHALLENGES.ADMIN_CUSTOM_PERIOD
            );
          }

          this.fetchSettings();
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
    switch (controlName) {
      case 'averageMaxCapacity':
        return 'Average Max Capacity';
      case 'language':
        return 'Language';
      case 'challengeRewardsCountForPeriod':
        return 'Challenge Rewards Count For Period';
      case 'periodOfRewardReset':
        return 'Period Of Reward Reset';
      case 'resetDayForRewards':
        return 'Reset Day For Rewards';
      case 'challengeInitiationDelayHours':
        return 'Challenge Initiation Delay Hours';
      case 'reservationHours':
        return 'Reservation Hours';
      case 'phoneNumber':
        return 'Phone Number';
      case 'clubName':
        return 'Club Name';
      case 'bonusTimeAfterReservationExpiration':
        return 'Bonus Time After Reservation Expiration';
      case 'isCustomPeriodOn':
        return 'Is Custom Period On';
      default:
        return controlName;
    }
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      id: new FormControl<number | null>(null),
      averageMaxCapacity: new FormControl<number | null>(null, [
        Validators.required,
      ]),
      language: new FormControl<string | null>('EN', Validators.required),
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
      reservationHours: [[]],
      phoneNumber: new FormControl<string | null>('', [Validators.required]),
      clubName: new FormControl<string | null>('', [Validators.required]),
      bonusTimeAfterReservationExpiration: new FormControl<number | null>(
        null,
        [Validators.required]
      ),
      isCustomPeriodOn: new FormControl<number>(0, [Validators.required]),
    });
  }
}
