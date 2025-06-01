import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-forbidden',
  templateUrl: 'forbidden.component.html',
  styleUrls: ['forbidden.component.scss'],
})
export class ForbiddenComponent {
  public imgPath = 'shared/assets/images/exceptions/forbidden-403.jpg';

  constructor(private readonly router: Router) {}

  public redirectTo() {
    this.router.navigateByUrl('/games/library');
  }
}
