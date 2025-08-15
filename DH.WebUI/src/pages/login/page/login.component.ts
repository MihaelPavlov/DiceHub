import { FrontEndLogService } from './../../../shared/services/frontend-log.service';
import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../../entities/auth/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
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
import { TenantSettingsService } from '../../../entities/common/api/tenant-settings.service';
import { LoadingService } from '../../../shared/services/loading.service';
import { LoadingInterceptorContextService } from '../../../shared/services/loading-context.service';
import { TranslateService } from '@ngx-translate/core';

interface ILoginForm {
  email: string;
  password: string;
  rememberMe: boolean;
}

@Component({
  selector: 'app-login',
  templateUrl: 'login.component.html',
  styleUrl: 'login.component.scss',
})
export class LoginComponent extends Form implements OnInit {
  override form: Formify<ILoginForm>;
  public showPassword = false;
  public getMessageFromRedirect: string | null = null;
  public showResend: boolean = false;
  public clubName: string | null = null;

  constructor(
    public override readonly toastService: ToastService,
    private readonly router: Router,
    private readonly authService: AuthService,
    private readonly messagingService: MessagingService,
    private readonly fb: FormBuilder,
    private readonly route: ActivatedRoute,
    private readonly tenantSettingsService: TenantSettingsService,
    private readonly loadingService: LoadingService,
    private readonly frontEndLogService: FrontEndLogService,
    private readonly loadingContext: LoadingInterceptorContextService,
    public override translateService: TranslateService
  ) {
    super(toastService, translateService);
    this.route.queryParams.subscribe((params) => {
      if (params['fromRegister'] === 'true') {
        this.getMessageFromRedirect =
          'Registration successful! Please check your email to confirm your account.';
      }

      if (params['fromForgotPassword'] === 'true') {
        this.getMessageFromRedirect =
          'Password reset email sent successfully! Please check your email.';
      }

      if (params['fromResetPassword'] === 'true') {
        this.getMessageFromRedirect =
          'Password reset was successfully! Please use your new password to sign in.';
      }

      if (params['fromCreateEmployeePassword'] === 'true') {
        this.getMessageFromRedirect =
          'Welcome to the team! Password creation was successfully! Please use your new password to sign in.';
      }
    });

    this.form = this.initFormGroup();
    this.form.valueChanges.subscribe(() => {
      if (this.getServerErrorMessage) {
        this.clearServerErrorMessage();
      }
      this.getMessageFromRedirect = null;
    });
  }

  public ngOnInit(): void {
    this.tenantSettingsService.getClubName().subscribe({
      next: (clubName) => {
        this.clubName = clubName;
      },
    });
  }

  public navigateToRegister(): void {
    this.router.navigateByUrl(ROUTE.REGISTER);
  }

  public navigateToForgotPassword(): void {
    this.router.navigateByUrl(ROUTE.FORGOT_PASSWORD);
  }

  public navigateToLanding(): void {
    this.router.navigateByUrl(ROUTE.LANDING);
  }

  private clearServerErrorMessage() {
    this.getServerErrorMessage = null;
  }

  protected override getControlDisplayName(controlName: string): string {
    switch (controlName) {
      case 'email':
        return 'Email';
      case 'password':
        return 'Password';
      default:
        return controlName;
    }
  }

  public resendConfirmationEmail(): void {
    if (this.form.controls.email.valid)
      this.authService
        .sendEmailConfirmationRequest(this.form.controls.email.value)
        .subscribe({
          next: (isSuccessfully) => {
            if (isSuccessfully && isSuccessfully === true) {
              this.toastService.success({
                message: 'Confirmation email sent successfully!',
                type: ToastType.Success,
              });
              this.clearServerErrorMessage();
              this.showResend = false;
            } else {
              this.toastService.error({
                message: 'Failed to send confirmation email.',
                type: ToastType.Error,
              });
            }
          },
          error: () => {
            this.toastService.error({
              message: 'Failed to send confirmation email.',
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

      this.authService
        .login({
          email: this.form.controls.email.value,
          password: this.form.controls.password.value,
          deviceToken,
          timeZone,
        })
        .subscribe({
          next: (response) => {
            if (response) {
              this.authService.authenticateUser(
                response.accessToken,
                response.refreshToken
              );

              this.router.navigateByUrl(FULL_ROUTE.GAMES.LIBRARY);
            }
          },
          error: (error) => {
            this.frontEndLogService
              .sendWarning(error.message, error.stack)
              .subscribe({
                next: (response) => {
                  console.log('Error logged successfully:', response);
                },
              });
            if (error.error.errors.Email)
              this.getServerErrorMessage = error.error.errors.Email[0];
            if (error.error.errors.EmailNotConfirmed) {
              this.getServerErrorMessage =
                error.error.errors.EmailNotConfirmed[0];
              this.showResend = true;
            }

            this.toastService.error({
              message: AppToastMessage.SomethingWrong,
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

  public async loginUser(): Promise<void> {
    try {
      this.loadingService.loadingOn();
      let deviceToken: string | null = null;
      if (this.messagingService.isPushUnsupportedIOS()) {
        this.frontEndLogService
          .sendWarning(
            'Push notifications not supported on this iOS version',
            'On LoginComponent.onLogin()'
          )
          .subscribe();
      } else {
        deviceToken =
          await this.messagingService.getDeviceTokenForRegistration();
      }
      const timeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;

      this.authService
        .login({
          email: 'rap4obg@abv.bg',
          password: '1qaz!QAZ',
          deviceToken,
          timeZone,
        })
        .subscribe({
          next: (response) => {
            if (response) {
              this.authService.authenticateUser(
                response.accessToken,
                response.refreshToken
              );

              this.router.navigateByUrl('games/library');
            }
          },
          error: (error) => {
            this.handleServerErrors(error);
            this.toastService.error({
              message: AppToastMessage.SomethingWrong,
              type: ToastType.Error,
            });
          },
          complete: () => {
            this.loadingService.loadingOff();
          },
        });
    } catch (error: any) {
      console.error('Error during login:', error);
      alert(error.message);
      alert(error);
    }
  }

  public async loginAdmin() {
    this.loadingContext.enableManualMode();
    this.loadingService.loadingOn();
    let deviceToken: string | null = null;
    if (this.messagingService.isPushUnsupportedIOS()) {
      this.frontEndLogService
        .sendWarning(
          'Push notifications not supported on this iOS version',
          'On LoginComponent.onLogin()'
        )
        .subscribe();
    } else {
      this.frontEndLogService
        .sendWarning(
          'Start Getting device token for registration',
          'On LoginComponent.onLogin()'
        )
        .subscribe();
      deviceToken = await this.messagingService.getDeviceTokenForRegistration();
    }
    const timeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;

    this.frontEndLogService
      .sendWarning('login as admin', 'On LoginComponent.onLogin()')
      .subscribe();
    this.authService
      .login({
        email: 'sa@dicehub.com',
        password: '1qaz!QAZ',
        deviceToken,
        timeZone,
      })
      .subscribe({
        next: (response) => {
          if (response) {
            this.authService.authenticateUser(
              response.accessToken,
              response.refreshToken
            );

            this.router.navigateByUrl('games/library');
          }
        },
        error: (error) => {
          this.handleServerErrors(error);
          this.toastService.error({
            message: AppToastMessage.SomethingWrong,
            type: ToastType.Error,
          });
        },
        complete: () => {
          this.loadingService.loadingOff();
          this.loadingContext.disableManualMode();
        },
      });
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      email: new FormControl<string>('', [Validators.required]),
      password: new FormControl<string>('', Validators.required),
      rememberMe: new FormControl<boolean>(true),
    });
  }
}
