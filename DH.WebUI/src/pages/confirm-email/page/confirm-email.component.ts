import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../entities/auth/auth.service';
import { FULL_ROUTE, ROUTE } from '../../../shared/configs/route.config';
import { ToastService } from '../../../shared/services/toast.service';
import { ToastType } from '../../../shared/models/toast.model';
import { TenantSettingsService } from '../../../entities/common/api/tenant-settings.service';
import { TranslateService } from '@ngx-translate/core';
import { LanguageService } from '../../../shared/services/language.service';
import { ChallengeHubService } from '../../../entities/challenges/api/challenge-hub.service';
import { ChallengeOverlayService } from '../../../shared/services/challenges-overlay.service';

@Component({
  selector: 'app-confirm-email',
  templateUrl: 'confirm-email.component.html',
  styleUrl: 'confirm-email.component.scss',
})
export class ConfirmEmailComponent implements OnInit {
  public readonly confirming = this.translateService.instant(
    'confirm_email.confirming'
  );
  public message = this.confirming;
  public isSuccess = false;
  private email: string | null = null;
  public clubName: string | null = null;

  constructor(
    private readonly toastService: ToastService,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly authService: AuthService,
    private readonly tenantSettingsService: TenantSettingsService,
    private readonly translateService: TranslateService,
    private readonly languageService: LanguageService,
    private readonly challengeOverlayService: ChallengeOverlayService,
    private readonly challengeHubService: ChallengeHubService
  ) {}

  public ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      this.email = params['email'];
      const token = params['token'];
      const language = params['language'];
      if (this.email && token && language) {
        console.log(`Language from query param: ${language}`);

        this.languageService.setLanguage(language);
        this.confirmEmail(this.email, token);
      } else {
        this.message = this.translateService.instant(
          'confirm_email.invalid_confirm_link'
        );
      }
    });

    this.tenantSettingsService.getClubName().subscribe({
      next: (clubName) => {
        this.clubName = clubName;
      },
    });
  }

  public navigateToLogin(): void {
    this.router.navigateByUrl(ROUTE.LOGIN);
  }

  public resendConfirmationEmail(): void {
    if (this.email)
      this.authService
        .sendEmailConfirmationRequest(
          this.email,
          this.languageService.getCurrentLanguage()
        )
        .subscribe({
          next: (isSuccessfully) => {
            if (isSuccessfully && isSuccessfully === true) {
              this.toastService.success({
                message: this.translateService.instant(
                  'confirm_email.email_send_successfully'
                ),
                type: ToastType.Success,
              });
              this.message = this.translateService.instant(
                'confirm_email.email_send_successfully_message'
              );
              this.isSuccess = true;
            } else {
              this.toastService.error({
                message: this.translateService.instant(
                  'confirm_email.email_send_not_successfully'
                ),
                type: ToastType.Error,
              });
            }
          },
          error: () => {
            this.toastService.error({
              message: this.translateService.instant(
                'confirm_email.email_send_not_successfully'
              ),
              type: ToastType.Error,
            });
          },
        });
  }

  public confirmEmail(email: string, token: string): void {
    const timeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;

    this.authService.confirmEmail(email, token, timeZone).subscribe({
      next: (response) => {
        this.isSuccess = response ? true : false;

        if (!this.isSuccess)
          this.message = this.translateService.instant(
            'confirm_email.email_confirmation_failed'
          );
        else {
          this.message = this.translateService.instant(
            'confirm_email.email_confirmed_successfully'
          );

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

            if (this.challengeOverlayService.overlay.value) {
              this.challengeHubService.initChallengeHubConnection(
                response!.userId,
                this.challengeOverlayService.overlay.value
              );
            }

            this.authService.initiateNotifications(email);

            this.router.navigateByUrl(FULL_ROUTE.GAMES.LIBRARY);
          }, 3000);
        }
      },
      error: (error) => {
        if (error.error.errors.InvalidToken) {
          this.message = error.error.errors.InvalidToken[0];
        } else {
          this.message = this.translateService.instant(
            'confirm_email.email_confirmation_failed'
          );
        }
        this.isSuccess = false;
      },
    });
  }
}
