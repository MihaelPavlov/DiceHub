import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-unauthorized',
  templateUrl: 'unauthorized.component.html',
})
export class UnauthorizedComponent {
  public imgPath = 'assets/img/401_error.svg';
  public details = [
    'The access to this page is restricted.',
    'Please go to the Landing Page or refer to your system administrator.',
  ];

  constructor(private readonly router: Router) {}

  public redirectTo() {
    this.router.navigateByUrl('/games/library');
  }
}
