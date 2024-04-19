import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-find-meeple-manager',
  templateUrl: 'find-meeple-management.component.html',
  styleUrl: 'find-meeple-management.component.scss',
})
export class FindMeepleManagementComponent {
  constructor(private readonly router: Router) {}

  public navigateToCreateMeepleRoom(): void {
    this.router.navigateByUrl('meeples/create');
  }
}
