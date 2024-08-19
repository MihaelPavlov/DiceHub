import { Component, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { MenuTabsService } from '../../../shared/services/menu-tabs.service';
import { MENU_ITEM_LABELS } from '../../../shared/models/menu-items-labels.const';

@Component({
  selector: 'app-events-library',
  templateUrl: 'events-library.component.html',
  styleUrl: 'events-library.component.scss',
})
export class EventsLibraryComponent implements OnDestroy {
  constructor(
    private readonly router: Router,
    private readonly menuTabsService: MenuTabsService
  ) {
    this.menuTabsService.setActive(MENU_ITEM_LABELS.EVENTS);
  }
  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public navigateToEventDetails(): void {
    this.router.navigateByUrl('events/1/details');
  }
}
