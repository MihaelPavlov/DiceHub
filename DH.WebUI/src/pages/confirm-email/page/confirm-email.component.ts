import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../entities/auth/auth.service';
import { FULL_ROUTE } from '../../../shared/configs/route.config';
import { ToastService } from '../../../shared/services/toast.service';
import { ToastType } from '../../../shared/models/toast.model';
import { TenantSettingsService } from '../../../entities/common/api/tenant-settings.service';

@Component({
  selector: 'app-confirm-email',
  templateUrl: 'confirm-email.component.html',
  styleUrl: 'confirm-email.component.scss',
})
export class ConfirmEmailComponent implements OnInit {
  message = 'Confirming...';
  public isSuccess = false;
  private email: string | null = null;
  public clubName: string | null = null;

  constructor(
    private readonly toastService: ToastService,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly authService: AuthService,
    private readonly tenantSettingsService: TenantSettingsService
  ) {}

  public ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      this.email = params['email'];
      const token = params['token'];
      if (this.email && token) {
        this.confirmEmail(this.email, token);
      } else {
        this.message = 'Invalid confirmation link.';
      }
    });

    this.tenantSettingsService.get().subscribe({
      next: (settings) => {
        this.clubName = settings?.clubName;
      },
    });
  }

  public navigateToLogin(): void {
    this.router.navigateByUrl('login');
  }

  public resendConfirmationEmail(): void {
    if (this.email)
      this.authService.sendEmailConfirmationRequest(this.email).subscribe({
        next: (isSuccessfully) => {
          if (isSuccessfully && isSuccessfully === true) {
            this.toastService.success({
              message: 'Confirmation email sent successfully!',
              type: ToastType.Success,
            });
            this.message = 'Email sent successfully! Please check your inbox.';
            this.isSuccess = true;
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

  public confirmEmail(email: string, token: string): void {
    this.authService.confirmEmail(email, token).subscribe({
      next: (response) => {
        this.isSuccess = response ? true : false;

        if (!this.isSuccess) this.message = 'Email confirmation failed.';
        else {
          this.message = `Your email has been confirmed successfully! <br/>You will be redirected to your account in <span class="dot-loader">3</span> seconds...`;

          let countdown = 3;
          const dotLoader = setInterval(() => {
            countdown--;
            const loaderEl = document.querySelector('.dot-loader');
            if (loaderEl) loaderEl.textContent = countdown.toString();
            if (countdown === 0) {
              clearInterval(dotLoader);
            }
          }, 1000);

          setTimeout(() => {
            this.authService.authenticateUser(
              response!.accessToken,
              response!.refreshToken
            );

            this.authService.initiateNotifications(email);

            this.router.navigateByUrl(FULL_ROUTE.GAMES.LIBRARY);
          }, 3000);
        }
      },
      error: (error) => {
        console.log('error', error);

        this.isSuccess = false;
        this.message = 'Email confirmation failed.';
      },
    });
  }
}
