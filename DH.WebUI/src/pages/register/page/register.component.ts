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
    private readonly tenantSettingsService: TenantSettingsService
  ) {
    super(toastService);
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

  public async register(): Promise<void> {
    if (this.form.valid) {
      try {
        const deviceToken =
          await this.messagingService.getDeviceTokenForRegistration();

        this.authService
          .register({
            username: this.form.controls.username.value,
            email: this.form.controls.email.value,
            password: this.form.controls.password.value,
            confirmPassword: this.form.controls.confirmPassword.value,
            deviceToken: deviceToken ?? '',
          })
          .subscribe({
            next: (response) => {
              if (
                response &&
                response.isEmailConfirmationSendedSuccessfully === true &&
                response.isRegistrationSuccessfully === true
              ) {
                this.toastService.success({
                  message:
                    'Registration successful! Please check your email to confirm your account.',
                  type: ToastType.Success,
                });
                setTimeout(() => {
                  this.router.navigate(['/login'], {
                    queryParams: { fromRegister: 'true' },
                  });
                }, 5000);
              } else if (
                response &&
                response.isRegistrationSuccessfully === true &&
                response.isEmailConfirmationSendedSuccessfully === false
              ) {
                this.toastService.success({
                  message: `Registration successful, but we couldn't send the confirmation email. 
                    Please try resending it or contact support.`,
                  type: ToastType.Success,
                });
                this.getServerErrorMessage = `We couldn't send the confirmation email. 
                    Please try resending it or contact support.`;
                this.showResend = true;
              } else {
                this.toastService.error({
                  message: 'Registration failed. Please try again.',
                  type: ToastType.Error,
                });
              }
            },
            error: (error) => this.handleRegistrationError(error),
          });
      } catch (error) {
        // TODO: Handle the case where getting the device token fails
        // this.toastService.error({
        //   message: AppToastMessage.SomethingWrong,
        //   type: ToastType.Error,
        // });
      }
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
              setTimeout(() => {
                this.router.navigate(['/login'], {
                  queryParams: { fromRegister: 'true' },
                });
              }, 4000);
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

  private handleRegistrationError(error: any): void {
    if (error.error?.errors?.Exist) {
      this.getServerErrorMessage = error.error.errors.Exist[0];
    } else {
      this.handleServerErrors(error);
    }

    this.toastService.error({
      message: AppToastMessage.SomethingWrong,
      type: ToastType.Error,
    });
  }

  protected override getControlDisplayName(controlName: string): string {
    switch (controlName) {
      case 'email':
        return 'Email';
      case 'password':
        return 'Password';
      case 'username':
        return 'Username';
      case 'confirmPassword':
        return 'Confirm Password';
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
