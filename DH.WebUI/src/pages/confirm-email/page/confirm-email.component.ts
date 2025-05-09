import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../entities/auth/auth.service';
import { FULL_ROUTE } from '../../../shared/configs/route.config';

@Component({
  selector: 'app-confirm-email',
  templateUrl: 'confirm-email.component.html',
  styleUrl: 'confirm-email.component.scss',
})
export class ConfirmEmailComponent implements OnInit {
  message = 'Confirming...';
  isSuccess = false;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly authService: AuthService
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      const email = params['email'];
      const token = params['token'];
       if (email && token) {
        this.confirmEmail(email, token);
      } else {
        this.message = 'Invalid confirmation link.';
      }
    });
  }

  public navigateToLogin(): void {
    this.router.navigateByUrl('login');
  }

  confirmEmail(email: string, token: string) {
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
