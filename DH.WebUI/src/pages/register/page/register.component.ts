import { LoadingService } from './../../../shared/services/loading.service';
import { Component } from '@angular/core';
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
import { FULL_ROUTE, ROUTE } from '../../../shared/configs/route.config';
import { AppToastMessage } from '../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../shared/models/toast.model';

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
export class RegisterComponent extends Form {
  override form: Formify<IRegisterForm>;

  constructor(
    private readonly router: Router,
    private readonly authService: AuthService,
    private readonly messagingService: MessagingService,
    public override readonly toastService: ToastService,
    private readonly fb: FormBuilder,
    private readonly loadingService: LoadingService
  ) {
    super(toastService);
    this.form = this.initFormGroup();
    this.form.valueChanges.subscribe(() => {
      if (this.getServerErrorMessage) {
        this.clearServerErrorMessage();
      }
    });
  }

  private clearServerErrorMessage() {
    this.getServerErrorMessage = null;
  }

  public navigateToLogin(): void {
    this.router.navigateByUrl(ROUTE.LOGIN);
  }

  public register(): void {
    if (this.form.valid) {
      this.loadingService.loadingOn();
      this.messagingService
        .getDeviceTokenForRegistration()
        .then((deviceToken) => {
          this.authService
            .register({
              username: this.form.controls.username.value,
              email: this.form.controls.email.value,
              password: this.form.controls.password.value,
              confirmPAssword: this.form.controls.confirmPassword.value,
              deviceToken,
            })
            .subscribe({
              next: () => {
                this.loginAfterSuccessRegistration();
              },
              error: (error) => {
                if (
                  error.error &&
                  error.error.errors &&
                  error.error.errors.Exist
                ) {
                  this.getServerErrorMessage = error.error.errors.Exist[0];
                } else {
                  this.handleServerErrors(error);
                }
                this.toastService.error({
                  message: AppToastMessage.FailedToSaveChanges,
                  type: ToastType.Error,
                });
                this.loadingService.loadingOff();
              },
              complete: () => {
                this.loadingService.loadingOff();
              },
            });
        })
        .catch((err) => {
          //TODO: If we couldn't get the device token. Think of a flow, should we be able to register a user. Answer i think YES
          this.toastService.error({
            message: AppToastMessage.SomethingWrong,
            type: ToastType.Error,
          });

          this.loadingService.loadingOff();
        });
    }
  }

  private loginAfterSuccessRegistration(): void {
    this.authService
      .login({
        email: this.form.controls.email.value,
        password: this.form.controls.password.value,
      })
      .subscribe({
        next: (response) => {
          if (response) {
            this.authService.onnSuccessfullyLogin(
              response.accessToken,
              response.refreshToken
            );

            this.router.navigateByUrl(FULL_ROUTE.GAMES.LIBRARY);

            this.authService.initiateNotifications(
              this.form.controls.email.value
            );
          } else {
            this.toastService.error({
              message: AppToastMessage.SomethingWrong,
              type: ToastType.Error,
            });
          }
        },
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
