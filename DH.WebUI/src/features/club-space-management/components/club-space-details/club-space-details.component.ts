import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-club-space-details',
  templateUrl: 'club-space-details.component.html',
  styleUrl: 'club-space-details.component.scss',
})
export class ClubSpaceDetailsComponent {
  constructor(private readonly router: Router) {}

  public backToSpaceHome(): void {
    this.router.navigateByUrl('space/home');
  }

  public handleSearchExpression(searchExpression: string) {
  }
}
