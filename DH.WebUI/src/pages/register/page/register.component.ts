import { FrontEndLogService } from './../../../shared/services/frontend-log.service';
import { Component, OnInit } from '@angular/core';
import { Form } from '../../../shared/components/form/form.component';
import {
  FormBuilder,
  FormGroup,
  FormControl,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../entities/auth/auth.service';
import { MessagingService } from '../../../entities/messaging/api/messaging.service';
import { Formify } from '../../../shared/models/form.model';
import { ToastService } from '../../../shared/services/toast.service';
import { ROUTE } from '../../../shared/configs/route.config';
import { AppToastMessage } from '../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../shared/models/toast.model';
import { TenantSettingsService } from '../../../entities/common/api/tenant-settings.service';
import { LoadingService } from '../../../shared/services/loading.service';
import { LoadingInterceptorContextService } from '../../../shared/services/loading-context.service';
import { TranslateService } from '@ngx-translate/core';
import { LanguageService } from '../../../shared/services/language.service';

interface IRegisterForm {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
}

@Component({
  selector: 'app-register',
  templateUrl: 'register.component.html',
  styleUrl: 'register.component.scss',
})
export class RegisterComponent extends Form implements OnInit {
  override form: Formify<IRegisterForm>;

  public showPassword: boolean = false;
  public showConfirmPassword: boolean = false;
  public showResend: boolean = false;
  public clubName: string | null = null;

  constructor(
    private readonly router: Router,
    private readonly authService: AuthService,
    private readonly messagingService: MessagingService,
    public override readonly toastService: ToastService,
    private readonly fb: FormBuilder,
    private readonly loadingService: LoadingService,
    private readonly tenantSettingsService: TenantSettingsService,
    private readonly frontEndLogService: FrontEndLogService,
    private readonly loadingContext: LoadingInterceptorContextService,
    public override translateService: TranslateService,
    private readonly languageService: LanguageService
  ) {
    super(toastService, translateService);
    this.form = this.initFormGroup();
    this.form.valueChanges.subscribe(() => {
      if (this.getServerErrorMessage) {
        this.clearServerErrorMessage();
      }
    });
  }
  public ngOnInit(): void {
    this.tenantSettingsService.getClubName().subscribe({
      next: (clubName) => {
        this.clubName = clubName;
      },
    });
  }

  private clearServerErrorMessage() {
    this.getServerErrorMessage = null;
  }

  public navigateToLogin(): void {
    this.router.navigateByUrl(ROUTE.LOGIN);
  }

  public navigateToLanding(): void {
    this.router.navigateByUrl(ROUTE.LANDING);
  }

  private isPushUnsupportedIOS(): boolean {
    const ua = navigator.userAgent.toLowerCase();
    const isIOS = /iphone|ipad|ipod/.test(ua);
    const versionMatch = ua.match(/os (\d+)_/i); // e.g., "OS 16_3" becomes ["os 16_", "16"]
    const iosVersion = versionMatch ? parseFloat(versionMatch[1]) : 0;

    return isIOS && iosVersion < 16.4;
  }
  public async register(): Promise<void> {
    if (this.form.valid) {
      this.loadingContext.enableManualMode();
      this.loadingService.loadingOn();
      try {
        let deviceToken: string | null = null;
        if (this.isPushUnsupportedIOS()) {
          this.frontEndLogService
            .sendWarning(
              'Push notifications not supported on this iOS version',
              'On Register Page'
            )
            .subscribe();
        } else {
          deviceToken =
            await this.messagingService.getDeviceTokenForRegistration();
        }
        this.authService
          .register({
            username: this.form.controls.username.value,
            email: this.form.controls.email.value,
            password: this.form.controls.password.value,
            confirmPassword: this.form.controls.confirmPassword.value,
            deviceToken: deviceToken,
            language: this.languageService.getCurrentLanguage(),
          })
          .subscribe({
            next: (response) => {
              if (
                response &&
                response.isEmailConfirmationSendedSuccessfully === true &&
                response.isRegistrationSuccessfully === true
              ) {
                this.toastService.success({
                  message: this.translateService.instant(
                    'register.successfully_message'
                  ),
                  type: ToastType.Success,
                });
                setTimeout(() => {
                  this.router.navigate([ROUTE.LOGIN], {
                    queryParams: { fromRegister: 'true' },
                  });
                }, 5000);
              } else if (
                response &&
                response.isRegistrationSuccessfully === true &&
                response.isEmailConfirmationSendedSuccessfully === false
              ) {
                this.toastService.success({
                  message: this.translateService.instant(
                    'register.successfully_with_email_failed'
                  ),
                  type: ToastType.Success,
                });
                (this.getServerErrorMessage = this.translateService.instant(
                  'register.register_error_message'
                )),
                  (this.showResend = true);
              } else {
                this.toastService.error({
                  message: this.translateService.instant(
                    'register.registration_failed'
                  ),
                  type: ToastType.Error,
                });
              }
            },
            error: (error) => {
              this.loadingService.loadingOff();
              this.loadingContext.disableManualMode();
              this.handleRegistrationError(error);
            },
            complete: () => {
              this.loadingService.loadingOff();
              this.loadingContext.disableManualMode();
            },
          });
      } catch (error: any) {
        this.loadingService.loadingOff();
        this.loadingContext.disableManualMode();
        this.frontEndLogService
          .sendError(error.message, 'On Register Page')
          .subscribe();
        this.toastService.error({
          message: this.translateService.instant(
            AppToastMessage.SomethingWrong
          ),
          type: ToastType.Error,
        });
      }
    }
  }

  public resendConfirmationEmail(): void {
    if (this.form.controls.email.valid)
      this.authService
        .sendEmailConfirmationRequest(
          this.form.controls.email.value,
          this.languageService.getCurrentLanguage()
        )
        .subscribe({
          next: (isSuccessfully) => {
            if (isSuccessfully && isSuccessfully === true) {
              this.toastService.success({
                message: this.translateService.instant(
                  'register.confirmation_email_sent'
                ),
                type: ToastType.Success,
              });
              this.clearServerErrorMessage();
              setTimeout(() => {
                this.router.navigate([ROUTE.LOGIN], {
                  queryParams: { fromRegister: 'true' },
                });
              }, 4000);
            } else {
              this.toastService.error({
                message: this.translateService.instant(
                  'register.failed_email_send'
                ),
                type: ToastType.Error,
              });
            }
          },
          error: () => {
            this.toastService.error({
              message: this.translateService.instant(
                'register.failed_email_send'
              ),
              type: ToastType.Error,
            });
          },
        });
  }

  private handleRegistrationError(error: any): void {
    if (error.error?.errors?.Exist)
      this.getServerErrorMessage = error.error.errors.Exist[0];
    else if (error.error?.errors?.General)
      this.getServerErrorMessage = error.error.errors.General[0];
    else {
      this.handleServerErrors(error);
    }

    this.toastService.error({
      message: this.translateService.instant(AppToastMessage.SomethingWrong),
      type: ToastType.Error,
    });
  }

  protected override getControlDisplayName(controlName: string): string {
    switch (controlName) {
      case 'email':
        return this.translateService.instant(
          'register.control_display_names.email'
        );
      case 'password':
        return this.translateService.instant(
          'register.control_display_names.password'
        );
      case 'username':
        return this.translateService.instant(
          'register.control_display_names.username'
        );
      case 'confirmPassword':
        return this.translateService.instant(
          'register.control_display_names.confirm_password'
        );
      default:
        return controlName;
    }
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      username: new FormControl<string>('', [
        Validators.required,
        Validators.minLength(3),
      ]),
      email: new FormControl<string>('', [
        Validators.required,
        Validators.minLength(3),
      ]),
      password: new FormControl<string>('', Validators.required),
      confirmPassword: new FormControl<string>('', Validators.required),
    });
  }
}
