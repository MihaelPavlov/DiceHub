import { Component, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { MenuTabsService } from '../../../shared/services/menu-tabs.service';
import { MENU_ITEM_LABELS } from '../../../shared/models/menu-items-labels.const';

@Component({
  selector: 'app-find-meeple-manager',
  templateUrl: 'find-meeple-management.component.html',
  styleUrl: 'find-meeple-management.component.scss',
})
export class FindMeepleManagementComponent implements OnDestroy {
  constructor(
    private readonly router: Router,
    private readonly menuTabsService: MenuTabsService
  ) {
    this.menuTabsService.setActive(MENU_ITEM_LABELS.MEEPLE);
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public navigateToCreateMeepleRoom(): void {
    this.router.navigateByUrl('meeples/create');
  }
}
