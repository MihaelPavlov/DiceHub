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
    private readonly tenantSettingsService: TenantSettingsService
  ) {
    super(toastService);
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
      }
    });
  }

  public navigateToRegister(): void {
    this.router.navigateByUrl(ROUTE.REGISTER);
  }

  public navigateToForgotPassword(): void {
    this.router.navigateByUrl(ROUTE.FORGOT_PASSWORD);
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

  public navigateToGameDetails(): void {
    this.router.navigateByUrl('games/1/details');
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
      const deviceToken =
        await this.messagingService.getDeviceTokenForRegistration();

      this.authService
        .login({
          email: this.form.controls.email.value,
          password: this.form.controls.password.value,
          deviceToken,
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
          },
        });
    }
  }

  loginUser() {
    this.authService
      .login({
        email: 'rap4obg@abv.bg',
        password: '1qaz!QAZ',
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
            message: AppToastMessage.FailedToSaveChanges,
            type: ToastType.Error,
          });
        },
      });
  }

  loginUser2() {
    this.authService
      .login({
        email: 'rap4obg88@abv.bg',
        password: '1qaz!QAZ',
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
            message: AppToastMessage.FailedToSaveChanges,
            type: ToastType.Error,
          });
        },
      });
  }

  loginUser3() {
    this.authService
      .login({
        email: 'rap4obg4@abv.bg',
        password: '123456789Mm!',
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
            message: AppToastMessage.FailedToSaveChanges,
            type: ToastType.Error,
          });
        },
      });
  }
  loginUser4() {
    this.authService
      .login({
        email: 'rap4obg17@abv.bg',
        password: '123456789Mm!',
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
            message: AppToastMessage.FailedToSaveChanges,
            type: ToastType.Error,
          });
        },
      });
  }

  loginAdmin() {
    this.authService
      .login({
        email: 'sa@dicehub.com',
        password: '1qaz!QAZ',
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
            message: AppToastMessage.FailedToSaveChanges,
            type: ToastType.Error,
          });
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
