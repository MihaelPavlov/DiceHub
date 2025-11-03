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
import { FULL_ROUTE, ROUTE } from '../../../../shared/configs/route.config';
import { SupportLanguages } from '../../../../entities/common/models/support-languages.enum';
import { TenantUserSettingsService } from '../../../../entities/common/api/tenant-user-settings.service';
import { combineLatest, forkJoin, switchMap, tap } from 'rxjs';
import { IUserSettings } from '../../../../entities/common/models/user-settings.model';
import { LanguageService } from '../../../../shared/services/language.service';
import { TranslateService } from '@ngx-translate/core';
import { TranslateInPipe } from '../../../../shared/pipe/translate-in.pipe';

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
    public override translateService: TranslateService,
    private readonly translateInPipe: TranslateInPipe
  ) {
    super(toastService, translateService);
    this.form = this.initFormGroup();
    this.form.valueChanges.subscribe(() => {
      if (this.getServerErrorMessage) {
        this.clearServerErrorMessage();
      }
    });
    this.menuTabsService.setActive(NAV_ITEM_LABELS.PROFILE);
    this.initSelectValues();
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
          periodOfRewardReset:
            TimePeriodType[tenantSettings.periodOfRewardReset],
          resetDayForRewards: WeekDay[tenantSettings.resetDayForRewards],
          language:
            SupportLanguages[userSettings.language] ?? SupportLanguages.EN,
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
        if (this.userSettings)
          this.languageService.setLanguage(userSettings.language);
      },
    });
  }

  private clearServerErrorMessage() {
    this.getServerErrorMessage = null;
  }

  public onSave(): void {

    if (this.form.valid) {
      let oldLanguage;
      let newLanguage;
      const periodTranslation$ = this.translateInPipe.transform(
        `period_time_names.${
          TimePeriodType[this.form.controls.periodOfRewardReset.value]
        }`,
        SupportLanguages.EN.toString().toLowerCase()
      );

      const weekDayTranslation$ = this.translateInPipe.transform(
        `week_days_names.${
          WeekDay[this.form.controls.resetDayForRewards.value]
        }`,
        SupportLanguages.EN.toString().toLowerCase()
      );

      const languageTranslation$ = this.translateInPipe.transform(
        `languages_names.${
          SupportLanguages[this.form.controls.language.value]
        }`,
        SupportLanguages.EN.toLowerCase()
      );

      forkJoin([periodTranslation$, weekDayTranslation$, languageTranslation$])
        .pipe(
          switchMap(([periodName, weekDayName, language]) => {
            oldLanguage = this.languageService.getCurrentLanguage();
            newLanguage = language as unknown as SupportLanguages;
            const updatedSettings = {
              language: newLanguage,
              phoneNumber: this.form.controls.phoneNumber.value,
            };
            return combineLatest([
              this.tenantSettingsService.update({
                id: this.tenantSettingsId,
                averageMaxCapacity: this.form.controls.averageMaxCapacity.value,
                challengeRewardsCountForPeriod:
                  this.form.controls.challengeRewardsCountForPeriod.value,
                periodOfRewardReset: periodName as unknown as TimePeriodType,
                resetDayForRewards: weekDayName as unknown as WeekDay,
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
              this.tenantUserSettingsService.update(
                updatedSettings as IUserSettings
              ),
            ]);
          })
        )
        .subscribe({
          next: () => {
            if (newLanguage != oldLanguage) {
              this.languageService
                .setLanguage$(newLanguage as SupportLanguages)
                .subscribe({
                  next: () => {
                    this.initSelectValues();
                  },
                });
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

            this.toastService.success({
              message: this.translateService.instant(
                AppToastMessage.ChangesSaved
              ),
              type: ToastType.Success,
            });
          },
          error: (error) => {
            this.handleServerErrors(error);
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

  public backNavigateBtn() {
    this.router.navigateByUrl(ROUTE.PROFILE.CORE);
  }

  protected override getControlDisplayName(controlName: string): string {
    switch (controlName) {
      case 'averageMaxCapacity':
        return this.translateService.instant(
          'space_settings.control_display_names.average_max_capacity'
        );
      case 'language':
        return this.translateService.instant(
          'space_settings.control_display_names.language'
        );
      case 'challengeRewardsCountForPeriod':
        return this.translateService.instant(
          'space_settings.control_display_names.challenge_rewards_count_for_period'
        );
      case 'periodOfRewardReset':
        return this.translateService.instant(
          'space_settings.control_display_names.period_of_reward_reset'
        );
      case 'resetDayForRewards':
        return this.translateService.instant(
          'space_settings.control_display_names.reset_day_for_rewards'
        );
      case 'challengeInitiationDelayHours':
        return this.translateService.instant(
          'space_settings.control_display_names.challenge_initiation_delay_hours'
        );
      case 'reservationHours':
        return this.translateService.instant(
          'space_settings.control_display_names.reservation_hours'
        );
      case 'phoneNumber':
        return this.translateService.instant(
          'space_settings.control_display_names.phone_number'
        );
      case 'clubName':
        return this.translateService.instant(
          'space_settings.control_display_names.club_name'
        );
      case 'bonusTimeAfterReservationExpiration':
        return this.translateService.instant(
          'space_settings.control_display_names.bonus_time_after_reservation_expiration'
        );
      case 'isCustomPeriodOn':
        return this.translateService.instant(
          'space_settings.control_display_names.is_custom_period_on'
        );
      default:
        return controlName;
    }
  }

  private initSelectValues(): void {
    this.weekDaysValues = Object.entries(WeekDay)
      .filter(([key, value]) => typeof value === 'number')
      .map(([key, value]) => ({
        id: value as number,
        name: this.translateService.instant(`week_days_names.${key}`),
      }));

    this.periodTimeValues = Object.entries(TimePeriodType)
      .filter(([key, value]) => typeof value === 'number')
      .map(([key, value]) => ({
        id: value as number,
        name: this.translateService.instant(`period_time_names.${key}`),
      }));

    this.toggleStateValues = Object.entries(ToggleState)
      .filter(([key, value]) => typeof value === 'number')
      .map(([key, value]) => ({
        id: value as number,
        name: this.translateService.instant(`toggle_state_names.${key}`),
      }));

    this.languagesValues = Object.keys(SupportLanguages)
      .filter((key) => isNaN(Number(key)))
      .map((name) => ({
        id: SupportLanguages[name as keyof typeof SupportLanguages],
        name: this.translateService.instant(`languages_names.${name}`),
      })) as unknown as IDropdown[];
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      id: new FormControl<number | null>(null),
      averageMaxCapacity: new FormControl<number | null>(null, [
        Validators.required,
      ]),
      language: new FormControl<string | null>('0', Validators.required),
      challengeRewardsCountForPeriod: new FormControl<number | null>(null, [
        Validators.required,
      ]),
      periodOfRewardReset: new FormControl<string | null>(
        '0',
        Validators.required
      ),
      resetDayForRewards: new FormControl<string | null>(
        '0',
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
