import { Component, OnDestroy } from '@angular/core';
import { MenuTabsService } from '../../../../../shared/services/menu-tabs.service';
import { MENU_ITEM_LABELS } from '../../../../../shared/models/menu-items-labels.const';

@Component({
  selector: 'app-game-categories',
  templateUrl: 'game-categories.component.html',
  styleUrl: 'game-categories.component.scss',
})
export class GameCategoriesComponent implements OnDestroy {
  constructor(private readonly menuTabsService: MenuTabsService) {
    this.menuTabsService.setActive(MENU_ITEM_LABELS.GAMES);
  }
  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public handleSearchExpression(searchExpression: string) {
    console.log('from categories search');
  }
}
