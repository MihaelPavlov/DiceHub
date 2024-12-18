import { Component, OnDestroy } from '@angular/core';
import { MenuTabsService } from '../../../shared/services/menu-tabs.service';
import { NAV_ITEM_LABELS } from '../../../shared/models/nav-items-labels.const';
import { AuthService } from '../../../entities/auth/auth.service';
import { Router } from '@angular/router';
import { UserRole } from '../../../entities/auth/enums/roles.enum';

@Component({
  selector: 'app-profile',
  templateUrl: 'profile.component.html',
  styleUrl: 'profile.component.scss',
})
export class ProfileComponent implements OnDestroy {
  constructor(
    private readonly menuTabsService: MenuTabsService,
    private readonly authService: AuthService,
    private readonly router: Router
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.PROFILE);
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public onLogout(): void {
    this.authService.logout();
    this.router.navigateByUrl('login');
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

  public backNavigateBtn() {}
}
