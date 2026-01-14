import { Component, OnDestroy, OnInit } from '@angular/core';
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
import { GetOwnerStats } from '../../../entities/profile/models/get-owner-stats.interface';
import { TenantRouter } from '../../../shared/helpers/tenant-router';

@Component({
  selector: 'app-profile',
  templateUrl: 'profile.component.html',
  styleUrl: 'profile.component.scss',
  standalone: false,
})
export class ProfileComponent implements OnInit, OnDestroy {
  public username: string = this.authService.getUser?.username || '';
  public userStats!: Observable<GetUserStats>;
  public ownerStats!: Observable<GetOwnerStats>;

  constructor(
    private readonly menuTabsService: MenuTabsService,
    private readonly navigationService: NavigationService,
    private readonly authService: AuthService,
    private readonly usersService: UsersService,
    private readonly tenantRouter: TenantRouter,
    private readonly router: Router
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.PROFILE);
  }
  public ngOnInit(): void {
    if (this.isOwnerOrSuperAdmin) {
      this.ownerStats = this.usersService.getOwnerStats();
    } else {
      this.userStats = this.usersService.getUserStats();
    }
  }

  public get isOwnerOrSuperAdmin(): boolean {
    return (
      this.authService.getUser?.role === UserRole.Owner ||
      this.authService.getUser?.role === UserRole.SuperAdmin
    );
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public onLogout(): void {
    this.authService.logout().subscribe({
      next: () => this.tenantRouter.navigateTenant(ROUTE.LOGIN),
      error: () => this.tenantRouter.navigateTenant(ROUTE.LOGIN),
    });
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
      this.tenantRouter.navigateTenant(FULL_ROUTE.PROFILE.SETTINGS);
    else {
      this.tenantRouter.navigateTenant(FULL_ROUTE.PROFILE.USER_SETTINGS);
    }
  }

  public navigateToJobList(): void {
    this.tenantRouter.navigateTenant(FULL_ROUTE.PROFILE.JOBS);
  }

  public navigateToEmployeeList(): void {
    this.tenantRouter.navigateTenant(FULL_ROUTE.PROFILE.EMPLOYEES);
  }

  public navigateToOwnerDetails(): void {
    this.tenantRouter.navigateTenant(FULL_ROUTE.PROFILE.OWNER_DETAILS);
  }

  public navigateToInstructions(): void {
    this.navigationService.setPreviousUrl(this.router.url);
    this.tenantRouter.navigateTenant(ROUTE.INSTRUCTIONS);
  }

  public navigateToVisitorChart(): void {
    this.tenantRouter.navigateTenant(FULL_ROUTE.CHARTS.VISITORS);
  }

  public navigateToGameChart(): void {
    this.tenantRouter.navigateTenant(FULL_ROUTE.CHARTS.GAMES);
  }

  public navigateToReservationsChart(): void {
    this.tenantRouter.navigateTenant(FULL_ROUTE.CHARTS.RESERVATIONS);
  }

  public navigateToRewardCharts(): void {
    this.tenantRouter.navigateTenant(FULL_ROUTE.CHARTS.REWARDS);
  }
  public navigateToEventCharts(): void {
    this.tenantRouter.navigateTenant(FULL_ROUTE.CHARTS.EVENTS);
  }

  public navigateToChallengeLeaderboard(): void {
    this.tenantRouter.navigateTenant(FULL_ROUTE.CHARTS.CHALLENGES_LEADERBOARD);
  }

  public navigateToClubInfo(): void {
    this.tenantRouter.navigateTenant(FULL_ROUTE.PROFILE.CLUB_INFO);
  }

  // FUTURE Feature - Streaks Page
  //  public navigateToStreakLeaderboard(): void {
  //   this.tenantRouter.navigateTenant(FULL_ROUTE.CHARTS.STREAK_LEADERBOARD);
  // }
}
