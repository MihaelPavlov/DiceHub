import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { NavigationService } from '../../../shared/services/navigation-service';

@Component({
  selector: 'app-instruction-management',
  templateUrl: 'instruction-management.component.html',
  styleUrl: 'instruction-management.component.scss',
})
export class InstructionManagementComponent {
  constructor(
    private readonly router: Router,
    private readonly navigationService: NavigationService
  ) {}
  public defaultLinks = [
    { name: 'Reservations', path: '/instructions/reservation' },
    { name: 'Events', path: '/instructions/events' },
    { name: 'Challenges & Rewards', path: '/instructions/challenges' },
    { name: 'Meeples', path: '/instructions/meeples' },
  ];

  public navigateTo(path: string): void {
    this.navigationService.setPreviousUrl(this.router.url);
    this.router.navigateByUrl(path);
  }
}
