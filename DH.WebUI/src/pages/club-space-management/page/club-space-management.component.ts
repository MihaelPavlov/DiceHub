import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-club-space-management',
  templateUrl: 'club-space-management.component.html',
  styleUrl: 'club-space-management.component.scss',
})
export class ClubSpaceManagementComponent {
  constructor(private readonly router: Router) {}

  public navigateToSpaceManagement(): void {
    this.router.navigateByUrl('/space/list');
  }
}
