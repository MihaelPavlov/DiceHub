import { Component, OnInit } from '@angular/core';
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
import { TenantSettingsService } from '../../../entities/common/api/tenant-settings.service';
import { TranslateService } from '@ngx-translate/core';
import { LanguageService } from '../../../shared/services/language.service';
import { TenantRouter } from '../../../shared/helpers/tenant-router';

interface IForgotPasswordForm {
  email: string;
}

@Component({
  selector: 'app-forgot-password',
  templateUrl: 'forgot-password.component.html',
  styleUrl: 'forgot-password.component.scss',
  standalone: false,
})
export class ForgotPasswordComponent extends Form implements OnInit {
  override form: Formify<IForgotPasswordForm>;

  public clubName: string | null = null;

  constructor(
    private readonly tenantRouter: TenantRouter,
    private readonly router: Router,
    private readonly authService: AuthService,
    public override readonly toastService: ToastService,
    private readonly tenantSettingsService: TenantSettingsService,
    public override translateService: TranslateService,
    private readonly languageService: LanguageService,
    private readonly fb: FormBuilder
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
    this.tenantSettingsService.getClubName().subscribe({
      next: (clubName) => {
        this.clubName = clubName;
      },
    });
  }

  public navigateToLanding(): void {
    this.router.navigateByUrl(ROUTE.LANDING);
  }

  public onSubmit(): void {
    if (this.form.valid) {
      this.authService
        .forgotPassword(
          this.form.controls.email.value,
          this.languageService.getCurrentLanguage()
        )
        .subscribe({
          next: () => {
            this.toastService.success({
              type: ToastType.Success,
              message: this.translateService.instant(
                'forgot_password.send_was_successfully'
              ),
            });
            setTimeout(() => {
              this.router.navigate(
                [this.tenantRouter.buildTenantUrl(ROUTE.LOGIN)],
                {
                  queryParams: { fromForgotPassword: 'true' },
                }
              );
            }, 5000);
          },
          error: (error) => {
            if (error.error.errors.Email)
              this.getServerErrorMessage = error.error.errors.Email[0];
            this.toastService.error({
              type: ToastType.Error,
              message: this.translateService.instant(
                'forgot_password.send_was_unsuccessfully'
              ),
            });
          },
        });
    }
  }

  public navigateToLogin(): void {
    this.tenantRouter.navigateTenant(ROUTE.LOGIN);
  }

  protected override getControlDisplayName(controlName: string): string {
    switch (controlName) {
      case 'email':
        return this.translateService.instant(
          'forgot_password.control_display_names.email'
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
      email: new FormControl<string>('', [Validators.required]),
    });
  }
}
