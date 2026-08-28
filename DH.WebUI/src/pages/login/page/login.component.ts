import { TenantSettingsService } from './../../../entities/common/api/tenant-settings.service';
import { FrontEndLogService } from './../../../shared/services/frontend-log.service';
import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../../entities/auth/auth.service';
import { ActivatedRoute } from '@angular/router';
import { Form } from '../../../shared/components/form/form.component';
import { ToastService } from '../../../shared/services/toast.service';
import { Formify } from '../../../shared/models/form.model';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { AppToastMessage } from '../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../shared/models/toast.model';
import { FULL_ROUTE, ROUTE } from '../../../shared/configs/route.config';
import { MessagingService } from '../../../entities/messaging/api/messaging.service';
import { LoadingService } from '../../../shared/services/loading.service';
import { LoadingInterceptorContextService } from '../../../shared/services/loading-context.service';
import { TranslateService } from '@ngx-translate/core';
import { LanguageService } from '../../../shared/services/language.service';
import { ChallengeOverlayService } from '../../../shared/services/challenges-overlay.service';
import { ChallengeHubService } from '../../../entities/challenges/api/challenge-hub.service';
import { TenantRouter } from '../../../shared/helpers/tenant-router';
import { TenantContextService } from '../../../shared/services/tenant-context.service';

interface ILoginForm {
  email: string;
  password: string;
  rememberMe: boolean;
}

@Component({
  selector: 'app-login',
  templateUrl: 'login.component.html',
  styleUrl: 'login.component.scss',
  standalone: false,
})
export class LoginComponent extends Form implements OnInit {
  override form: Formify<ILoginForm>;
  public showPassword = false;
  public getMessageFromRedirect: string | null = null;
  public showResend: boolean = false;
  public clubName: string | null = null;
  public clubLogoUrl: string | null = null;
  public isAdminLogin = false;

  constructor(
    public override readonly toastService: ToastService,
    private readonly authService: AuthService,
    private readonly messagingService: MessagingService,
    private readonly fb: FormBuilder,
    private readonly route: ActivatedRoute,
    private readonly tenantRouter: TenantRouter,
    private readonly tenantContextService: TenantContextService,
    private readonly loadingService: LoadingService,
    private readonly frontEndLogService: FrontEndLogService,
    private readonly loadingContext: LoadingInterceptorContextService,
    public override translateService: TranslateService,
    private readonly languageService: LanguageService,
    private readonly challengeOverlayService: ChallengeOverlayService,
    private readonly challengeHubService: ChallengeHubService,
    private readonly tenantSettingsService: TenantSettingsService
  ) {
    super(toastService, translateService);

    this.route.queryParams.subscribe((params) => {
      if (params['fromRegister'] === 'true') {
        this.getMessageFromRedirect = this.translateService.instant(
          'login.from_message.register'
        );
      }

      if (params['fromForgotPassword'] === 'true') {
        this.getMessageFromRedirect = this.translateService.instant(
          'login.from_message.forgot_password'
        );
      }

      if (params['fromResetPassword'] === 'true') {
        this.getMessageFromRedirect = this.translateService.instant(
          'login.from_message.reset_password'
        );
      }

      if (params['fromCreateEmployeePassword'] === 'true') {
        this.getMessageFromRedirect = this.translateService.instant(
          'login.from_message.create_employee_password'
        );
      }
    });

    this.form = this.initFormGroup();
    this.form.valueChanges.subscribe(() => {
      if (this.getServerErrorMessage) {
        this.clearServerErrorMessage();
      }
      if (this.form.dirty) this.getMessageFromRedirect = null;
    });
  }

  public ngOnInit(): void {
    this.isAdminLogin = window.location.pathname.startsWith('/admin/login');

    if (this.isAdminLogin) {
      this.clubName = 'DiceHub Admin';
      return;
    }

    if (this.tenantContextService.tenantName) {
      this.clubName = this.tenantContextService.tenantName;
    }

    // Always resolve club branding: the club's owner may have uploaded a
    // custom logo, which replaces the default DiceHub mark on the login card.
    this.tenantSettingsService.getClubName().subscribe({
      next: (res) => {
        this.clubName ??= res.clubName;
        this.clubLogoUrl = this.resolveClubLogoUrl(res.logoFileName);
      },
    });
  }

  private resolveClubLogoUrl(logoFileName: string | null): string | null {
    if (!logoFileName) return null;

    if (/^https?:\/\//i.test(logoFileName)) {
      return logoFileName;
    }

    return `/shared/assets/images/tenant_logos/${logoFileName}`;
  }

  public changeClub(): void {
    this.tenantContextService.clearTenant();
    this.tenantRouter.navigateGlobal(ROUTE.CHOOSE_CLUB);
  }

  public navigateToRegister(): void {
    this.tenantRouter.navigateGlobal(ROUTE.REGISTER);
  }

  public navigateToForgotPassword(): void {
    this.tenantRouter.navigateGlobal(ROUTE.FORGOT_PASSWORD);
  }

  public navigateToLanding(): void {
    this.tenantRouter.navigateGlobal(ROUTE.LANDING);
  }

  private clearServerErrorMessage() {
    this.getServerErrorMessage = null;
  }

  /**
   * This app's login button is type="button" (not "submit"), and the whole
   * flow runs through Angular's HttpClient rather than a real HTML form POST -
   * so the browser/WebView's native "detect a form submission, offer to save
   * the password" heuristic never has anything to trigger on. The Credential
   * Management API (supported by Chrome/Edge on desktop and Android, which
   * covers both browser/PWA use and this app's Capacitor WebView) triggers
   * the same native save-password prompt explicitly instead. No-op on
   * browsers without support (e.g. Safari) or if the user declines - either
   * way it must never affect the login flow itself.
   */
  private async savePasswordCredential(email: string, password: string): Promise<void> {
    const PasswordCredentialCtor = (window as any).PasswordCredential;
    if (!('credentials' in navigator) || !PasswordCredentialCtor) {
      return;
    }

    try {
      const credential = new PasswordCredentialCtor({
        id: email,
        password,
        name: email,
      });
      await (navigator.credentials as any).store(credential);
    } catch {
      // Best-effort only.
    }
  }

  protected override getControlDisplayName(controlName: string): string {
    switch (controlName) {
      case 'email':
        return this.translateService.instant(
          'login.control_display_names.email'
        );
      case 'password':
        return this.translateService.instant(
          'login.control_display_names.password'
        );
      default:
        return controlName;
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
                  'login.toast_messages.resend_successfully'
                ),
                type: ToastType.Success,
              });
              this.clearServerErrorMessage();
              this.showResend = false;
            } else {
              this.toastService.error({
                message: this.translateService.instant(
                  'login.toast_messages.resend_not_successfully'
                ),
                type: ToastType.Error,
              });
            }
          },
          error: () => {
            this.toastService.error({
              message: this.translateService.instant(
                'login.toast_messages.resend_not_successfully'
              ),
              type: ToastType.Error,
            });
          },
        });
  }

  public async onLogin(): Promise<void> {
    if (this.form.valid) {
      this.loadingContext.enableManualMode();
      this.loadingService.loadingOn();
      let deviceToken: string | null = null;
      if (this.messagingService.isPushUnsupportedIOS()) {
        console.log('Not supported on this iOS version');

        this.frontEndLogService
          .sendWarning(
            'Push notifications not supported on this iOS version',
            'On LoginComponent.onLogin()'
          )
          .subscribe();
      } else {
        console.log('Start Getting device token for login');

        deviceToken =
          await this.messagingService.getDeviceTokenForRegistration();
        console.log('device-token', deviceToken);
      }

      const timeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;

      const loginEmail = this.form.controls.email.value;
      const loginPassword = this.form.controls.password.value;

      this.authService
        .login({
          email: loginEmail,
          password: loginPassword,
          tenantId: this.isAdminLogin ? null : this.tenantContextService.tenantId,
          deviceToken,
          timeZone,
        })
        .subscribe({
          next: (response) => {
            if (response) {
              this.authService.authenticateUser(
                response.accessToken,
                response.refreshToken,
                response.tenantId,
                !!this.form.controls.rememberMe.value
              );
              this.savePasswordCredential(loginEmail, loginPassword);

              if (
                response.tenantId !== 'system' &&
                this.challengeOverlayService.overlay.value
              ) {
                this.challengeHubService.initChallengeHubConnection(
                  response.userId,
                  this.challengeOverlayService.overlay.value
                );
              }

              if (response.tenantId === 'system') {
                this.tenantContextService.clearTenant();
                this.tenantRouter.navigateGlobal(['admin', 'applicants']);
              } else {
                this.tenantContextService.tenantId = response.tenantId;
                this.tenantRouter.navigateTenant(FULL_ROUTE.GAMES.LIBRARY);
              }
            }
          },
          error: (error) => {
            this.frontEndLogService
              .sendWarning(error.message, error.stack)
              .subscribe();
            if (error.error.errors.Email)
              this.getServerErrorMessage = error.error.errors.Email[0];
            if (error.error.errors.EmailNotConfirmed) {
              this.getServerErrorMessage =
                error.error.errors.EmailNotConfirmed[0];
              this.showResend = true;
            }
            if (error.error.errors.TenantId) {
              this.getServerErrorMessage = this.translateService.instant(
                'login.errors.tenant_mismatch'
              );
            }

            this.toastService.error({
              message: this.translateService.instant(
                AppToastMessage.SomethingWrong
              ),
              type: ToastType.Error,
            });

            this.loadingService.loadingOff();
            this.loadingContext.disableManualMode();
          },
          complete: () => {
            this.loadingService.loadingOff();
            this.loadingContext.disableManualMode();
          },
        });
    }
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      email: new FormControl<string>('', [Validators.required]),
      password: new FormControl<string>('', Validators.required),
      rememberMe: new FormControl<boolean>(true),
    });
  }
}
