import { Component, OnDestroy } from '@angular/core';
import { MenuTabsService } from '../../../shared/services/menu-tabs.service';
import { NAV_ITEM_LABELS } from '../../../shared/models/nav-items-labels.const';
import { AuthService } from '../../../entities/auth/auth.service';
import { Router } from '@angular/router';

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

  public navigateToSettings():void{
    this.router.navigateByUrl("profile/settings");
  }

  public backNavigateBtn() {}
}
