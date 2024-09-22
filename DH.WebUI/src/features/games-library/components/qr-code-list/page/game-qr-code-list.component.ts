import { Component, OnDestroy } from '@angular/core';
import { MenuTabsService } from '../../../../../shared/services/menu-tabs.service';
import { NAV_ITEM_LABELS } from '../../../../../shared/models/nav-items-labels.const';
import { Router } from '@angular/router';

@Component({
  selector: 'app-game-qr-code-list',
  templateUrl: 'game-qr-code-list.component.html',
  styleUrl: 'game-qr-code-list.component.scss',
})
export class GameQrCodeListComponent implements OnDestroy {
  public isMenuVisible: boolean = false;

  constructor(
    private readonly menuTabsService: MenuTabsService,
    private readonly router: Router
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.GAMES);
  }
  
  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public backNavigateBtn() {
    //TODO: Adjust the id
    this.router.navigateByUrl('games/1/details');
  }

  public showMenu(): void {
    this.isMenuVisible = !this.isMenuVisible;
  }
}
