import { map } from 'rxjs';
import { Component } from '@angular/core';
import { AuthService } from '../../../entities/auth/auth.service';
import { Router } from '@angular/router';
import { MessagingService } from '../../../entities/messaging/api/messaging.service';

@Component({
  selector: 'app-login',
  templateUrl: 'login.component.html',
  styleUrl: 'login.component.scss',
})
export class LoginComponent {
  constructor(
    private readonly router: Router,
    readonly authService: AuthService,
    private readonly messagingService: MessagingService
  ) {}

  public navigateToGameDetails(): void {
    this.router.navigateByUrl('games/1/details');
  }

  loginUser() {
    this.authService.login({
      email: 'rap4obg2@abv.bg',
      password: '123456789Mm!',
    });
  }

  loginUser2() {
    this.authService.login({
      email: 'rap4obg3@abv.bg',
      password: '123456789Mm!',
    });
  }

  loginUser3() {
    this.authService.login({
      email: 'rap4obg4@abv.bg',
      password: '123456789Mm!',
    });
  }
  loginUser4() {
    this.authService.login({
      email: 'rap4obg17@abv.bg',
      password: '123456789Mm!',
    },true);
  }

  loginAdmin() {
    this.authService.login({ email: 'sa@dicehub.com', password: '1qaz!QAZ' });
  }
  game() {
    this.authService.game({ name: 'test123' });
  }
  register() {
    this.messagingService
      .getDeviceTokenForRegistration()
      .then((deviceToken) => {
        console.log('from register -> ', deviceToken);

        this.authService
          .register({
            username: 'rap4obg17',
            email: 'rap4obg17@abv.bg',
            password: '123456789Mm!',
            deviceToken,
          })
          .subscribe({ next: () => {
            this.loginUser4();
          } });
      });
  }
  userInfo() {
    this.authService.userinfo();
  }
  logout() {
    this.authService.logout();
  }
}
