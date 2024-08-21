import { Component, OnDestroy, OnInit } from '@angular/core';
import { MenuTabsService } from '../../../../../shared/services/menu-tabs.service';
import { MENU_ITEM_LABELS } from '../../../../../shared/models/menu-items-labels.const';
import { GameCategoriesService } from '../../../../../entities/games/api/game-categories.service';
import { IGameCategory } from '../../../../../entities/games/models/game-category.model';
import { Router } from '@angular/router';
import { SearchService } from '../../../../../shared/services/search.service';

@Component({
  selector: 'app-game-categories',
  templateUrl: 'game-categories.component.html',
  styleUrl: 'game-categories.component.scss',
})
export class GameCategoriesComponent implements OnInit, OnDestroy {
  public categories: IGameCategory[] = [];

  constructor(
    private readonly gameCategoriesService: GameCategoriesService,
    private readonly menuTabsService: MenuTabsService,
    private readonly searchService: SearchService,
    private readonly router: Router
  ) {
    this.menuTabsService.setActive(MENU_ITEM_LABELS.GAMES);
  }

  public ngOnInit(): void {
    this.fetchGameList();
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
    this.searchService.hideSearchForm();
  }

  public handleSearchExpression(searchExpression: string) {
    this.fetchGameList(searchExpression);
  }

  public navigateToGameLibrary(id: number): void {
    this.router.navigateByUrl(`games/library/${id}`);
  }

  private fetchGameList(searchExpression: string = '') {
    this.gameCategoriesService
      .getList(searchExpression)
      .subscribe((categories) => (this.categories = categories ?? []));
  }
}
