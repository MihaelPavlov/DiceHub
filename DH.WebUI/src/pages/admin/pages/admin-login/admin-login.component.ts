import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { filter, take } from 'rxjs';
import { JwtHelperService } from '@auth0/angular-jwt';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from '../../../../entities/auth/auth.service';
import { UserRole } from '../../../../entities/auth/enums/roles.enum';
import { Form } from '../../../../shared/components/form/form.component';
import { Formify } from '../../../../shared/models/form.model';
import { ToastService } from '../../../../shared/services/toast.service';
import { ToastType } from '../../../../shared/models/toast.model';
import { ROUTE } from '../../../../shared/configs/route.config';

interface IAdminLoginForm {
  email: string;
  password: string;
}

@Component({
  selector: 'app-admin-login',
  templateUrl: './admin-login.component.html',
  styleUrl: './admin-login.component.scss',
  standalone: false,
})
export class AdminLoginComponent extends Form implements OnInit {
  override form: Formify<IAdminLoginForm>;
  public showPassword = false;

  constructor(
    private readonly fb: FormBuilder,
    private readonly router: Router,
    private readonly authService: AuthService,
    private readonly jwtHelper: JwtHelperService,
    public override readonly toastService: ToastService,
    public override readonly translateService: TranslateService
  ) {
    super(toastService, translateService);
    this.form = this.initFormGroup();
    this.form.valueChanges.subscribe(() => {
      if (this.getServerErrorMessage) this.getServerErrorMessage = null;
    });
  }

  public ngOnInit(): void {
    const token = localStorage.getItem('jwt');
    if (token && !this.jwtHelper.isTokenExpired(token)) {
      const user = this.authService.getUser;
      if (user?.role === UserRole.SuperAdmin) {
        this.router.navigate([`/${ROUTE.ADMIN.PROVISION}`]);
      }
    }
  }

  public onLogin(): void {
    if (!this.form.valid) return;

    const timeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;

    this.authService
      .login({
        email: this.form.controls.email.value,
        password: this.form.controls.password.value,
        tenantId: null,
        deviceToken: null,
        timeZone,
      })
      .subscribe({
        next: (response) => {
          if (!response) return;

          this.authService.authenticateUser(
            response.accessToken,
            response.refreshToken
          );

          this.authService.userInfo$
            .pipe(
              filter((user) => user !== null),
              take(1)
            )
            .subscribe((user) => {
              if (user?.role === UserRole.SuperAdmin) {
                this.router.navigate([`/${ROUTE.ADMIN.PROVISION}`]);
              } else {
                this.getServerErrorMessage =
                  'Access denied. Super Admin role required.';
                this.authService.logout().subscribe();
              }
            });
        },
        error: (error) => {
          this.handleServerErrors(error);
          this.toastService.error({
            message: 'Login failed. Check your credentials.',
            type: ToastType.Error,
          });
        },
      });
  }

  public navigateToLanding(): void {
    this.router.navigate([ROUTE.LANDING]);
  }

  protected override getControlDisplayName(controlName: string): string {
    const names: Record<string, string> = {
      email: 'Email',
      password: 'Password',
    };
    return names[controlName] ?? controlName;
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      email: new FormControl<string>('', [Validators.required]),
      password: new FormControl<string>('', [Validators.required]),
    });
  }
}
