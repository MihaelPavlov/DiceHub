import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-not-found',
  templateUrl: 'not-found.component.html',
  styleUrls: ['not-found.component.scss'],
})
export class NotFoundComponent {
  public imgPath = 'shared/assets/images/exceptions/not-found-404.jpg';

  constructor(private readonly router: Router) {}

  public redirectTo() {
    this.router.navigateByUrl('/games/library');
  }
}
