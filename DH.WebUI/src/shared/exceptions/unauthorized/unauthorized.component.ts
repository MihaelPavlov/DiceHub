import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-unauthorized',
  templateUrl: 'unauthorized.component.html',
  styleUrls: ['unauthorized.component.scss'],
})
export class UnauthorizedComponent {
   public imgPath = 'shared/assets/images/exceptions/unauthorized-401.jpg';

  constructor(private readonly router: Router) {}

  public redirectTo() {
    this.router.navigateByUrl('/games/library');
  }
}
