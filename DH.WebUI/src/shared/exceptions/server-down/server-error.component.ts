import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../entities/auth/auth.service';

@Component({
  selector: 'app-server-error',
  templateUrl: 'server-error.component.html',
  styleUrls: ['server-error.component.scss'],
})
export class ServerErrorComponent {
  public imgPath = 'shared/assets/images/exceptions/server-error-500.jpg';

  constructor(
    private readonly router: Router,
    private readonly authService: AuthService
  ) {}

  public redirectTo() {
    if (this.authService.getUser) this.router.navigateByUrl('/games/library');
    else this.router.navigateByUrl('/login');
  }
}
