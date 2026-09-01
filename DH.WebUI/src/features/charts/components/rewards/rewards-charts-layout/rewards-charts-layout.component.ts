import { Component, OnDestroy } from '@angular/core';
import { MenuTabsService } from '../../../../../shared/services/menu-tabs.service';
import { NAV_ITEM_LABELS } from '../../../../../shared/models/nav-items-labels.const';
import { FULL_ROUTE, ROUTE } from '../../../../../shared/configs/route.config';
import { TenantRouter } from '../../../../../shared/helpers/tenant-router';

@Component({
  selector: 'rewards-layout-chart',
  templateUrl: 'rewards-charts-layout.component.html',
  styleUrl: 'rewards-charts-layout.component.scss',
  standalone: false,
})
export class RewardChartsLayoutComponent implements OnDestroy {
  constructor(
    private readonly menuTabsService: MenuTabsService,
    private readonly tenantRouter: TenantRouter
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.PROFILE);
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public backNavigateBtn(): void {
    this.tenantRouter.navigateTenant(ROUTE.PROFILE.CORE);
  }

  public navigateToExpiredCollectedChart(): void {
    this.tenantRouter.navigateTenant(
      FULL_ROUTE.CHARTS.REWARDS_EXPIRED_COLLECTED
    );
  }

  public navigateToCollectedRewardsChart(): void {
    this.tenantRouter.navigateTenant(FULL_ROUTE.CHARTS.REWARDS_COLLECTED);
  }
}
