import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-forbidden',
  templateUrl: 'forbidden.component.html',
})
export class ForbiddenComponent {
  public imgPath = 'assets/img/403_error.svg';
  public details = [
    'The access to this page is restricted.',
    'Please go to the Landing Page or refer to your system administrator.',
  ];

  constructor(private readonly router: Router) {}

  public redirectTo() {
    this.router.navigateByUrl('/games/library');
  }
}
