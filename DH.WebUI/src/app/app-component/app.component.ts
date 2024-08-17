import { Component } from '@angular/core';
import { AuthService } from '../../entities/auth/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  title = 'DH.WebUI';

  constructor(private readonly authService: AuthService) {
    if (!this.authService.getUser) {
      this.authService.userinfo();
    }
  }
}
