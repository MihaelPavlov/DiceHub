import { Component, OnDestroy } from "@angular/core";
import { NAV_ITEM_LABELS } from "../../../../../shared/models/nav-items-labels.const";
import { MenuTabsService } from "../../../../../shared/services/menu-tabs.service";
import { TenantRouter } from "../../../../../shared/helpers/tenant-router";

@Component({
    selector: 'events-layout-chart',
    templateUrl: 'events-charts-layout.component.html',
    styleUrl: 'events-charts-layout.component.scss',
    standalone: false
})
export class EventsChartsLayoutComponent implements OnDestroy {
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
    this.tenantRouter.navigateTenant('profile');
  }

  public navigateToEventsByDates(): void {
    this.tenantRouter.navigateTenant('charts/events/by-dates');
  }

  public navigateToEventsByEventIds(): void {
    this.tenantRouter.navigateTenant('charts/events/by-events');
  }
}
