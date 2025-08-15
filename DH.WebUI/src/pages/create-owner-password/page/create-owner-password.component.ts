import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormControl,
  Validators,
} from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../../entities/auth/auth.service';
import { TenantSettingsService } from '../../../entities/common/api/tenant-settings.service';
import { Form } from '../../../shared/components/form/form.component';
import { AppToastMessage } from '../../../shared/components/toast/constants/app-toast-messages.constant';
import { Formify } from '../../../shared/models/form.model';
import { ToastType } from '../../../shared/models/toast.model';
import { ToastService } from '../../../shared/services/toast.service';
import { ROUTE } from '../../../shared/configs/route.config';
import { TranslateService } from '@ngx-translate/core';

interface ICreateOwnerPasswordForm {
  clubPhoneNumber: string;
  newPassword: string;
  confirmPassword: string;
}

@Component({
  selector: 'app-create-owner-password',
  templateUrl: 'create-owner-password.component.html',
  styleUrl: 'create-owner-password.component.scss',
})
export class CreateOwnerPasswordComponent extends Form implements OnInit {
  override form: Formify<ICreateOwnerPasswordForm>;

  public showNewPassword = false;
  public showConfirmPassword = false;
  public email: string | null = null;
  public token: string | null = null;
  public clubName: string | null = null;
  public showResend: boolean = false;

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

  public navigateToLanding(): void {
    this.router.navigateByUrl(ROUTE.LANDING);
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
        .createOwnerPassword({
          email: this.email,
          clubPhoneNumber: this.form.controls.clubPhoneNumber.value,
          newPassword: this.form.controls.newPassword.value,
          confirmPassword: this.form.controls.confirmPassword.value,
          token: this.token,
        })
        .subscribe({
          next: () => {
            this.toastService.success({
              type: ToastType.Success,
              message: 'Password creation was successfully!',
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

            if (error.error.errors.ClubPhoneNumber)
              this.getServerErrorMessage =
                error.error.errors.ClubPhoneNumber[0];

            if (error.error.errors.InvalidToken) {
              this.getServerErrorMessage = error.error.errors.InvalidToken[0];
              this.showResend = true;
            }

            this.toastService.error({
              type: ToastType.Error,
              message: 'Password creation was unsuccessfully!',
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

  public resendPasswordEmail(): void {
    if (this.email)
      this.authService.sendOwnerPasswordResetRequest(this.email).subscribe({
        next: (isSuccessfully) => {
          if (isSuccessfully && isSuccessfully === true) {
            this.toastService.success({
              message:
                'Email sent! Please allow a few minutes for it to arrive in your inbox.',
              type: ToastType.Success,
            });
            this.clearServerErrorMessage();
            this.showResend = false;
          } else {
            this.toastService.error({
              message: 'Failed to send email. Please try again later.',
              type: ToastType.Error,
            });
          }
        },
        error: () => {
          this.toastService.error({
            message: 'Failed to send email. Please try again later.',
            type: ToastType.Error,
          });
        },
      });
  }

  protected override getControlDisplayName(controlName: string): string {
    switch (controlName) {
      case 'clubPhoneNumber':
        return 'Club Phone Number';
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
      clubPhoneNumber: new FormControl<string>('', [Validators.required]),
      newPassword: new FormControl<string>('', [Validators.required]),
      confirmPassword: new FormControl<string>('', [Validators.required]),
    });
  }
}
