import { Component, OnDestroy, OnInit } from '@angular/core';
import { MenuTabsService } from '../../../../../shared/services/menu-tabs.service';
import { NAV_ITEM_LABELS } from '../../../../../shared/models/nav-items-labels.const';
import { GameCategoriesService } from '../../../../../entities/games/api/game-categories.service';
import { IGameCategory } from '../../../../../entities/games/models/game-category.model';
import { Router } from '@angular/router';
import { SearchService } from '../../../../../shared/services/search.service';
import { FULL_ROUTE } from '../../../../../shared/configs/route.config';

@Component({
    selector: 'app-game-categories',
    templateUrl: 'game-categories.component.html',
    styleUrl: 'game-categories.component.scss',
    standalone: false
})
export class GameCategoriesComponent implements OnInit, OnDestroy {
  public categories: IGameCategory[] = [];

  constructor(
    private readonly gameCategoriesService: GameCategoriesService,
    private readonly menuTabsService: MenuTabsService,
    private readonly searchService: SearchService,
    private readonly router: Router
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.GAMES);
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
    this.router.navigateByUrl(FULL_ROUTE.GAMES.LIBRARY_BY_CATEGORY_ID(id));
  }

  private fetchGameList(searchExpression: string = '') {
    this.gameCategoriesService
      .getList(searchExpression)
      .subscribe((categories) => (this.categories = categories ?? []));
  }
}
