import { Component, OnDestroy, OnInit } from '@angular/core';
import { Form } from '../../../../shared/components/form/form.component';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { MenuTabsService } from '../../../../shared/services/menu-tabs.service';
import { ToastService } from '../../../../shared/services/toast.service';
import { NAV_ITEM_LABELS } from '../../../../shared/models/nav-items-labels.const';
import { Formify } from '../../../../shared/models/form.model';
import { TenantUserSettingsService } from '../../../../entities/common/api/tenant-user-settings.service';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../../shared/models/toast.model';
import { SupportLanguages } from '../../../../entities/common/models/support-languages.enum';
import { IDropdown } from '../../../../shared/models/dropdown.model';
import { LanguageService } from '../../../../shared/services/language.service';
import { TranslateService } from '@ngx-translate/core';
import { ROUTE } from '../../../../shared/configs/route.config';
import { TranslateInPipe } from '../../../../shared/pipe/translate-in.pipe';
import { forkJoin, switchMap } from 'rxjs';
import { IUserSettings } from '../../../../entities/common/models/user-settings.model';
import { UiTheme } from '../../../../shared/enums/ui-theme.enum';
import { ThemeService } from '../../../../shared/services/theme.service';
import { TenantRouter } from '../../../../shared/helpers/tenant-router';

interface IUserSettingsForm {
  phoneNumber: string;
  language: string;
  uiTheme: string;
}

@Component({
  selector: 'app-user-settings',
  templateUrl: 'user-settings.component.html',
  styleUrl: 'user-settings.component.scss',
  standalone: false,
})
export class UserSettingsComponent extends Form implements OnInit, OnDestroy {
  override form: Formify<IUserSettingsForm>;
  public tenantSettingsId: number | null = null;
  public languagesValues: IDropdown[] = [];
  public themeValues: IDropdown[] = [];
  public userSettings: IUserSettings | null = null;

  constructor(
    private readonly fb: FormBuilder,
    public override readonly toastService: ToastService,
    private readonly menuTabsService: MenuTabsService,
    private readonly userSettingService: TenantUserSettingsService,
    private readonly tenantRouter: TenantRouter,
    private readonly languageService: LanguageService,
    public override translateService: TranslateService,
    private readonly translateInPipe: TranslateInPipe,
    private readonly themeService: ThemeService
  ) {
    super(toastService, translateService);
    this.form = this.initFormGroup();
    this.form.valueChanges.subscribe(() => {
      if (this.getServerErrorMessage) {
        this.clearServerErrorMessage();
      }
    });
    this.menuTabsService.setActive(NAV_ITEM_LABELS.PROFILE);

    this.initDropdownValues();
  }

  private initDropdownValues(): void {
    this.languagesValues = Object.keys(SupportLanguages)
      .filter((key) => isNaN(Number(key)))
      .map((name) => ({
        id: SupportLanguages[name as keyof typeof SupportLanguages],
        name: this.translateService.instant(`languages_names.${name}`),
      })) as unknown as IDropdown[];

    this.themeValues = Object.keys(UiTheme)
      .filter((key) => isNaN(Number(key)))
      .map((name) => ({
        id: UiTheme[name as keyof typeof UiTheme],
        name: this.translateService.instant(`ui_theme_names.${name}`),
      })) as unknown as IDropdown[];
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public ngOnInit(): void {
    this.fetchSettings();
  }

  public fetchSettings(): void {
    this.userSettingService.get().subscribe({
      next: (userSettings) => {
        this.tenantSettingsId = userSettings.id ?? null;
        this.form.patchValue({
          phoneNumber: userSettings.phoneNumber,
          language: SupportLanguages[userSettings.language],
          uiTheme: UiTheme[userSettings.uiTheme],
        });

        this.userSettings = userSettings;
      },
    });
  }

  public onSave(): void {
    if (this.form.valid) {
      let oldLanguage;
      let newLanguage;

      let oldTheme;
      let newTheme;
      const languageTranslation$ = this.translateInPipe.transform(
        `languages_names.${
          SupportLanguages[this.form.controls.language.value]
        }`,
        SupportLanguages.EN.toLowerCase()
      );

      const themeTranslation$ = this.translateInPipe.transform(
        `ui_theme_names.${UiTheme[this.form.controls.uiTheme.value]}`,
        SupportLanguages.EN.toLowerCase()
      );

      forkJoin([languageTranslation$, themeTranslation$])
        .pipe(
          switchMap(([language, theme]) => {
            oldLanguage = this.languageService.getCurrentLanguage();
            newLanguage = language as unknown as SupportLanguages;

            oldTheme = this.userSettings?.uiTheme;
            newTheme = theme as unknown as UiTheme;

            return this.userSettingService.update({
              id: this.tenantSettingsId,
              phoneNumber: this.form.controls.phoneNumber.value,
              language: newLanguage,
              uiTheme: newTheme,
            });
          })
        )
        .subscribe({
          next: () => {
            if (newLanguage != oldLanguage) {
              this.languageService
                .setLanguage$(newLanguage as SupportLanguages)
                .subscribe({
                  next: () => {
                    this.initDropdownValues();
                  },
                });
            }
            if (newTheme != oldTheme) {
              this.themeService.applyTheme(newTheme as UiTheme);
              this.initDropdownValues();
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
    this.tenantRouter.navigateTenant(ROUTE.PROFILE.CORE);
  }

  protected override getControlDisplayName(controlName: string): string {
    switch (controlName) {
      case 'phoneNumber':
        return this.translateService.instant(
          'user_settings.controls_display_names.phone_number'
        );
      case 'language':
        return this.translateService.instant(
          'user_settings.controls_display_names.language'
        );
      case 'uiTheme':
        return this.translateService.instant(
          'user_settings.controls_display_names.uiTheme'
        );
      default:
        return controlName;
    }
  }

  private clearServerErrorMessage(): void {
    this.getServerErrorMessage = null;
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      id: new FormControl<number | null>(null),
      phoneNumber: new FormControl<string | null>('', [Validators.required]),
      language: new FormControl<string | null>('0', [Validators.required]),
      uiTheme: new FormControl<string | null>('0', [Validators.required]),
    });
  }
}
