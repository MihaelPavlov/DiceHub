import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-server-error',
  templateUrl: 'server-error.component.html',
  styleUrls: ['server-error.component.scss'],
})
export class ServerErrorComponent {
   public imgPath = 'shared/assets/images/exceptions/server-error-500.jpg';

  constructor(private readonly router: Router) {}

  public redirectTo() {
    this.router.navigateByUrl('/games/library');
  }
}
