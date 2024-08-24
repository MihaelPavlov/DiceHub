import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-not-found',
  templateUrl: 'not-found.component.html',
})
export class NotFoundComponent {
  public imgPath = 'assets/img/404_error.svg';
  public details = [
    'The object was not found.',
    'Please go to the Landing Page or refer to your system administrator.',
  ];

  constructor(private readonly router: Router) {}

  public redirectTo() {
    this.router.navigateByUrl('/games/library');
  }
}
