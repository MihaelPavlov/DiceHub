import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { GamesService } from '../../../entities/games/api/games.service';
import { IGameListResult } from '../../../entities/games/models/game-list.model';
import { MenuTabsService } from '../../../shared/services/menu-tabs.service';
import { MENU_ITEM_LABELS } from '../../../shared/models/menu-items-labels.const';
import { SearchService } from '../../../shared/services/search.service';

@Component({
  selector: 'app-games-library',
  templateUrl: 'games-library.component.html',
  styleUrl: 'games-library.component.scss',
})
export class GamesLibraryComponent implements OnInit, OnDestroy {
  public games: IGameListResult[] = [];
  private categoryId: number | null = null;

  constructor(
    private readonly router: Router,
    private readonly activeRoute: ActivatedRoute,
    private readonly gameService: GamesService,
    private readonly menuTabsService: MenuTabsService,
    private readonly searchService: SearchService
  ) {
    this.menuTabsService.setActive(MENU_ITEM_LABELS.GAMES);
  }
  
  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
    this.searchService.hideSearchForm();
  }

  public ngOnInit(): void {
    this.activeRoute.params.subscribe((params: Params) => {
      this.categoryId = params['id'];

      if (this.categoryId) {
        this.fetchGameListByCategoryId(this.categoryId);
      }
    });

    if (!this.categoryId) {
      this.fetchGameList();
    }
  }

  public navigateToGameDetails(id: number): void {
    this.router.navigateByUrl(`games/${id}/details`);
  }

  public handleSearchExpression(searchExpression: string) {
    if (this.categoryId) {
      this.fetchGameListByCategoryId(this.categoryId, searchExpression);
    } else {
      this.fetchGameList(searchExpression);
    }
  }

  private fetchGameListByCategoryId(id: number, searchExpression: string = '') {
    this.gameService
      .getListByCategoryId(id, searchExpression)
      .subscribe((gameList) => (this.games = gameList ?? []));
  }

  private fetchGameList(searchExpression: string = '') {
    this.gameService
      .getList(searchExpression)
      .subscribe((gameList) => (this.games = gameList ?? []));
  }
}
