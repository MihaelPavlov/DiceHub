import { Component, OnInit } from '@angular/core';
import { Formify } from '../../../shared/models/form.model';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../entities/auth/auth.service';
import { Form } from '../../../shared/components/form/form.component';
import { ToastService } from '../../../shared/services/toast.service';
import { ToastType } from '../../../shared/models/toast.model';
import { ROUTE } from '../../../shared/configs/route.config';
import { AppToastMessage } from '../../../shared/components/toast/constants/app-toast-messages.constant';
import { TenantSettingsService } from '../../../entities/common/api/tenant-settings.service';
import { TranslateService } from '@ngx-translate/core';

interface IResetPasswordForm {
  newPassword: string;
  confirmPassword: string;
}

@Component({
  selector: 'app-reset-password',
  templateUrl: 'reset-password.component.html',
  styleUrl: 'reset-password.component.scss',
})
export class ResetPasswordComponent extends Form implements OnInit {
  override form: Formify<IResetPasswordForm>;

  public showNewPassword = false;
  public showConfirmPassword = false;
  public email: string | null = null;
  public token: string | null = null;
  public clubName: string | null = null;

  constructor(
    private readonly router: Router,
    private readonly route: ActivatedRoute,
    private readonly authService: AuthService,
    public override readonly toastService: ToastService,
    private readonly fb: FormBuilder,
    private readonly tenantSettingsService: TenantSettingsService,
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

  public ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      this.email = params['email'];
      this.token = params['token'];
    });
    this.tenantSettingsService.getClubName().subscribe({
      next: (clubName) => {
        this.clubName = clubName;
      },
    });
  }

  public toggleNewPasswordVisibility(): void {
    this.showNewPassword = !this.showNewPassword;
  }

  public toggleConfirmPasswordVisibility(): void {
    this.showConfirmPassword = !this.showConfirmPassword;
  }

  public onSubmit(): void {
    if (this.form.valid && this.email && this.token) {
      this.authService
        .resetPassword({
          newPassword: this.form.controls.newPassword.value,
          confirmPassword: this.form.controls.confirmPassword.value,
          token: this.token,
          email: this.email,
        })
        .subscribe({
          next: () => {
            this.toastService.success({
              type: ToastType.Success,
              message: 'Password reset was successfully!',
            });
            setTimeout(() => {
              this.router.navigate([ROUTE.LOGIN], {
                queryParams: { fromResetPassword: 'true' },
              });
            }, 4000);
          },
          error: (error) => {
            if (error.error.errors.Password)
              this.getServerErrorMessage = error.error.errors.Password[0];

            this.toastService.error({
              type: ToastType.Error,
              message: 'Password reset was unsuccessfully!',
            });
          },
        });
    } else {
      this.toastService.error({
        type: ToastType.Error,
        message: AppToastMessage.SomethingWrong,
      });
    }
  }

  protected override getControlDisplayName(controlName: string): string {
    switch (controlName) {
      case 'newPassword':
        return 'New Password';
      case 'confirmPassword':
        return 'Confirm Password';
      default:
        return controlName;
    }
  }

  private clearServerErrorMessage(): void {
    this.getServerErrorMessage = null;
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      newPassword: new FormControl<string>('', [Validators.required]),
      confirmPassword: new FormControl<string>('', [Validators.required]),
    });
  }
}
