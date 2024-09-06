import { Component } from '@angular/core';
import { AuthService } from '../../../entities/auth/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: 'login.component.html',
  styleUrl: 'login.component.scss',
})
export class LoginComponent {
  constructor(
    private readonly router: Router,
    readonly authService: AuthService
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

  loginAdmin() {
    this.authService.login({ email: 'sa@dicehub.com', password: '1qaz!QAZ' });
  }
  game() {
    this.authService.game({ name: 'test123' });
  }
  register() {
    this.authService.register({
      username: 'rap4obg3',
      email: 'rap4obg3@abv.bg',
      password: '123456789Mm!',
    });
  }
  userInfo() {
    this.authService.userinfo();
  }
  logout() {
    this.authService.logout();
  }
}
