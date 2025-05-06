import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { NavigationService } from '../../../shared/services/navigation-service';
import { AuthService } from '../../../entities/auth/auth.service';

@Component({
  selector: 'app-instruction-management',
  templateUrl: 'instruction-management.component.html',
  styleUrl: 'instruction-management.component.scss',
})
export class InstructionManagementComponent {
  public defaultLinks = [
    { name: 'Reservations', path: '/instructions/reservation' },
    { name: 'Events', path: '/instructions/events' },
    { name: 'Challenges & Rewards', path: '/instructions/challenges' },
    { name: 'Meeples', path: '/instructions/meeples' },
  ];

  constructor(
    private readonly router: Router,
    private readonly navigationService: NavigationService,
    private readonly authService: AuthService
  ) {}

  public get isUserAuthenticated(): boolean {
    return this.authService.getUser !== null;
  }

  public navigateTo(path: string): void {
    this.navigationService.setPreviousUrl(this.router.url);
    this.router.navigateByUrl(path);
  }

  public backNavigateBtn() {
    this.router.navigateByUrl('profile');
  }
}
