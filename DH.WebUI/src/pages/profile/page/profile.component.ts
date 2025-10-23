import { Component, OnDestroy } from '@angular/core';
import { MenuTabsService } from '../../../shared/services/menu-tabs.service';
import { NAV_ITEM_LABELS } from '../../../shared/models/nav-items-labels.const';
import { AuthService } from '../../../entities/auth/auth.service';
import { Router } from '@angular/router';
import { UserRole } from '../../../entities/auth/enums/roles.enum';
import { UsersService } from '../../../entities/profile/api/user.service';
import { GetUserStats } from '../../../entities/profile/models/get-user-stats.interface';
import { Observable } from 'rxjs';
import { NavigationService } from '../../../shared/services/navigation-service';
import { FULL_ROUTE, ROUTE } from '../../../shared/configs/route.config';

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
    private readonly navigationService: NavigationService,
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
    this.router.navigateByUrl(ROUTE.LOGIN);
  }

  public isDifferentFromUser(): boolean {
    return this.authService.getUser?.role !== UserRole.User;
  }

  public isOwner(): boolean {
    return this.authService.getUser?.role === UserRole.Owner;
  }

  public isSuperAdmin(): boolean {
    return this.authService.getUser?.role === UserRole.SuperAdmin;
  }

  public navigateToSettings(): void {
    if (this.authService.getUser?.role === UserRole.Owner)
      this.router.navigateByUrl(FULL_ROUTE.PROFILE.SETTINGS);
    else {
      this.router.navigateByUrl(FULL_ROUTE.PROFILE.USER_SETTINGS);
    }
  }

  public navigateToJobList(): void {
    this.router.navigateByUrl(FULL_ROUTE.PROFILE.JOBS);
  }

  public navigateToEmployeeList(): void {
    this.router.navigateByUrl(FULL_ROUTE.PROFILE.EMPLOYEES);
  }

  public navigateToOwnerDetails(): void {
    this.router.navigateByUrl(FULL_ROUTE.PROFILE.OWNER_DETAILS);
  }

  public navigateToInstructions(): void {
    this.navigationService.setPreviousUrl(this.router.url);
    this.router.navigateByUrl(ROUTE.INSTRUCTIONS);
  }

  public navigateToVisitorChart(): void {
    this.router.navigateByUrl(FULL_ROUTE.CHARTS.VISITORS);
  }

  public navigateToGameChart(): void {
    this.router.navigateByUrl(FULL_ROUTE.CHARTS.GAMES);
  }

  public navigateToReservationsChart(): void {
    this.router.navigateByUrl(FULL_ROUTE.CHARTS.RESERVATIONS);
  }

  public navigateToRewardCharts(): void {
    this.router.navigateByUrl(FULL_ROUTE.CHARTS.REWARDS);
  }
  public navigateToEventCharts(): void {
    this.router.navigateByUrl(FULL_ROUTE.CHARTS.EVENTS);
  }

  public navigateToChallengeLeaderboard(): void {
    this.router.navigateByUrl(FULL_ROUTE.CHARTS.CHALLENGES_LEADERBOARD);
  }

  // FUTURE Feature - Streaks Page
  //  public navigateToStreakLeaderboard(): void {
  //   this.router.navigateByUrl(FULL_ROUTE.CHARTS.STREAK_LEADERBOARD);
  // }
}
