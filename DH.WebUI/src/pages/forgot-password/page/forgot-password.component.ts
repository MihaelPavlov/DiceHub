import { Component } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../entities/auth/auth.service';
import { Form } from '../../../shared/components/form/form.component';
import { Formify } from '../../../shared/models/form.model';
import { ToastService } from '../../../shared/services/toast.service';
import { ROUTE } from '../../../shared/configs/route.config';
import { ToastType } from '../../../shared/models/toast.model';

interface IForgotPasswordForm {
  email: string;
}

@Component({
  selector: 'app-forgot-password',
  templateUrl: 'forgot-password.component.html',
  styleUrl: 'forgot-password.component.scss',
})
export class ForgotPasswordComponent extends Form {
  override form: Formify<IForgotPasswordForm>;
  constructor(
    private readonly router: Router,
    private readonly authService: AuthService,
    public override readonly toastService: ToastService,
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

  public onSubmit(): void {
    if (this.form.valid) {
      this.authService
        .forgotPassword(this.form.controls.email.value)
        .subscribe({
          next: () => {
            this.toastService.success({
              type: ToastType.Success,
              message: 'Password reset email sent successfully!',
            });
            setTimeout(() => {
              this.router.navigate([ROUTE.LOGIN], {
                queryParams: { fromForgotPassword: 'true' },
              });
            }, 5000);
          },
          error: (error) => {
            if (error.error.errors.Email)
              this.getServerErrorMessage = error.error.errors.Email[0];
            this.toastService.error({
              type: ToastType.Error,
              message: 'Password reset email was not successfully!',
            });
          },
        });
    }
  }

  public navigateToLogin(): void {
    this.router.navigateByUrl(ROUTE.LOGIN);
  }

  protected override getControlDisplayName(controlName: string): string {
    switch (controlName) {
      case 'email':
        return 'Email';
      default:
        return controlName;
    }
  }

  private clearServerErrorMessage(): void {
    this.getServerErrorMessage = null;
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      email: new FormControl<string>('', [Validators.required]),
    });
  }
}
