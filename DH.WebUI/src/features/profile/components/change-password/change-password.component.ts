import { Component } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { Form } from '../../../../shared/components/form/form.component';
import { Formify } from '../../../../shared/models/form.model';
import { AuthService } from '../../../../entities/auth/auth.service';
import { ToastService } from '../../../../shared/services/toast.service';
import { ToastType } from '../../../../shared/models/toast.model';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';

interface IChangePasswordForm {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

@Component({
  selector: 'app-change-password',
  templateUrl: 'change-password.component.html',
  styleUrl: 'change-password.component.scss',
  standalone: false,
})
export class ChangePasswordComponent extends Form {
  override form: Formify<IChangePasswordForm>;

  public showCurrentPassword = false;
  public showNewPassword = false;
  public showConfirmPassword = false;
  public isSaving = false;

  constructor(
    private readonly fb: FormBuilder,
    private readonly authService: AuthService,
    public override readonly toastService: ToastService,
    public override translateService: TranslateService
  ) {
    super(toastService, translateService);
    this.form = this.initFormGroup();
    this.form.valueChanges.subscribe(() => {
      if (this.getServerErrorMessage) {
        this.clearServerErrorMessage();
      }
    });
  }

  public onSubmit(): void {
    if (!this.form.valid) return;

    this.isSaving = true;
    this.authService
      .changePassword({
        currentPassword: this.form.controls.currentPassword.value,
        newPassword: this.form.controls.newPassword.value,
        confirmPassword: this.form.controls.confirmPassword.value,
      })
      .subscribe({
        next: () => {
          this.toastService.success({
            type: ToastType.Success,
            message: this.translateService.instant(
              'change_password.toast_messages.changed_successfully'
            ),
          });
          this.form.reset();
          this.isSaving = false;
        },
        error: (error) => {
          if (error.error.errors.CurrentPassword)
            this.getServerErrorMessage = error.error.errors.CurrentPassword[0];
          else if (error.error.errors.ConfirmPassword)
            this.getServerErrorMessage = error.error.errors.ConfirmPassword[0];
          else if (error.error.errors.User)
            this.getServerErrorMessage = error.error.errors.User[0];

          this.toastService.error({
            type: ToastType.Error,
            message: this.translateService.instant(
              AppToastMessage.SomethingWrong
            ),
          });
          this.isSaving = false;
        },
      });
  }

  protected override getControlDisplayName(controlName: string): string {
    switch (controlName) {
      case 'currentPassword':
        return this.translateService.instant(
          'change_password.control_display_names.current_password'
        );
      case 'newPassword':
        return this.translateService.instant(
          'change_password.control_display_names.new_password'
        );
      case 'confirmPassword':
        return this.translateService.instant(
          'change_password.control_display_names.confirm_password'
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
      currentPassword: new FormControl<string>('', [Validators.required]),
      newPassword: new FormControl<string>('', [Validators.required]),
      confirmPassword: new FormControl<string>('', [Validators.required]),
    });
  }
}
