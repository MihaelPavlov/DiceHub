import { Component, OnDestroy } from '@angular/core';
import { MenuTabsService } from '../../../shared/services/menu-tabs.service';
import { NAV_ITEM_LABELS } from '../../../shared/models/nav-items-labels.const';

@Component({
  selector: 'app-profile',
  templateUrl: 'profile.component.html',
  styleUrl: 'profile.component.scss',
})
export class ProfileComponent implements OnDestroy {
  constructor(private readonly menuTabsService: MenuTabsService) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.PROFILE);
  }
  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }
  public backNavigateBtn() {}
}
