import { Component, OnDestroy } from '@angular/core';
import { MenuTabsService } from '../../../shared/services/menu-tabs.service';
import { NAV_ITEM_LABELS } from '../../../shared/models/nav-items-labels.const';
import { AuthService } from '../../../entities/auth/auth.service';
import { Router } from '@angular/router';
import { UserRole } from '../../../entities/auth/enums/roles.enum';
import { UsersService } from '../../../entities/profile/api/user.service';
import { GetUserStats } from '../../../entities/profile/models/get-user-stats.interface';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-profile',
  templateUrl: 'profile.component.html',
  styleUrl: 'profile.component.scss',
})
export class ProfileComponent implements OnDestroy {
  public username: string = this.authService.getUser?.username || '';
  public userStats!: Observable<GetUserStats>;
  
  constructor(
    private readonly menuTabsService: MenuTabsService,
    private readonly authService: AuthService,
    private readonly usersService: UsersService,
    private readonly router: Router
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.PROFILE);

    this.userStats = this.usersService.getUserStats();
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public onLogout(): void {
    this.authService.logout();
    this.router.navigateByUrl('login');
  }

  public isAdmin(): boolean {
    return this.authService.getUser?.role !== UserRole.User;
  }

  public navigateToSettings(): void {
    if (this.authService.getUser?.role === UserRole.Owner)
      this.router.navigateByUrl('profile/settings');
    else {
      this.router.navigateByUrl('profile/user-settings');
    }
  }

  public navigateToEmployeeList(): void {
    this.router.navigateByUrl('profile/employees');
  }

  public navigateToVisitorChart(): void {
    this.router.navigateByUrl('charts/visitors');
  }

  public navigateToReservationsChart(): void {
    this.router.navigateByUrl('charts/reservations');
  }

  public navigateToRewardCharts(): void {
    this.router.navigateByUrl('charts/rewards');
  }
  public navigateToEventCharts(): void {
    this.router.navigateByUrl('charts/events');
  }

  public navigateToChallengeLeaderboard(): void {
    this.router.navigateByUrl('charts/challenges/leaderboard');
  }

  public backNavigateBtn() {}
}
