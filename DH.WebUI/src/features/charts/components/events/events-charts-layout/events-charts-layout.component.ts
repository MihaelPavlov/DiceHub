import { Component, OnDestroy } from "@angular/core";
import { Router } from "@angular/router";
import { ControlsMenuComponent } from "../../../../../shared/components/menu/controls-menu.component";
import { NAV_ITEM_LABELS } from "../../../../../shared/models/nav-items-labels.const";
import { MenuTabsService } from "../../../../../shared/services/menu-tabs.service";

@Component({
  selector: 'events-layout-chart',
  templateUrl: 'events-charts-layout.component.html',
  styleUrl: 'events-charts-layout.component.scss',
})
export class EventsChartsLayoutComponent implements OnDestroy {
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

  public showMenu(event: MouseEvent, controlMenu: ControlsMenuComponent): void {
    event.stopPropagation();
    controlMenu.toggleMenu();
  }

  public navigateToEventsByDates(): void {
    this.router.navigateByUrl('charts/events/by-dates');
  }

  public navigateToEventsByEventIds(): void {
    this.router.navigateByUrl('charts/events/by-events');
  }
}
