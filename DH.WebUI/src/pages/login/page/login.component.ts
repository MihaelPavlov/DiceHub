import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../../entities/auth/auth.service';
import { Router } from '@angular/router';
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
export class LoginComponent extends Form {
  override form: Formify<ILoginForm>;
  constructor(
    private readonly router: Router,
    readonly authService: AuthService,
    public override readonly toastService: ToastService,
    private readonly messagingService: MessagingService,
    private readonly fb: FormBuilder
  ) {
    super(toastService);
    this.form = this.initFormGroup();
    this.form.valueChanges.subscribe(() => {
      if (this.getServerErrorMessage) {
        this.clearServerErrorMessage();
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
              this.authService.onnSuccessfullyLogin(
                response.accessToken,
                response.refreshToken
              );

              this.router.navigateByUrl(FULL_ROUTE.GAMES.LIBRARY);
            }
          },
          error: (error) => {
            this.getServerErrorMessage = 'Email or Password is invalid';
            this.toastService.error({
              message: AppToastMessage.FailedToSaveChanges,
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
            this.authService.onnSuccessfullyLogin(
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
            this.authService.onnSuccessfullyLogin(
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
            this.authService.onnSuccessfullyLogin(
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
            this.authService.onnSuccessfullyLogin(
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
            this.authService.onnSuccessfullyLogin(
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
