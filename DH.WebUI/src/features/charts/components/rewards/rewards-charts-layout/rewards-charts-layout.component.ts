import { Component, OnDestroy } from '@angular/core';
import { MenuTabsService } from '../../../../../shared/services/menu-tabs.service';
import { Router } from '@angular/router';
import { NAV_ITEM_LABELS } from '../../../../../shared/models/nav-items-labels.const';

@Component({
  selector: 'rewards-layout-chart',
  templateUrl: 'rewards-charts-layout.component.html',
  styleUrl: 'rewards-charts-layout.component.scss',
})
export class RewardChartsLayoutComponent implements OnDestroy {
  public isMenuVisible: boolean = false;

  constructor(
    private readonly menuTabsService: MenuTabsService,
    private readonly router: Router
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.PROFILE);
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public backNavigateBtn(): void {
    this.router.navigateByUrl('profile');
  }

  public showMenu(): void {
    this.isMenuVisible = !this.isMenuVisible;
  }

  public navigateToExpiredCollectedChart():void{
    this.router.navigateByUrl('charts/rewards/expired-collected')
  }
}
