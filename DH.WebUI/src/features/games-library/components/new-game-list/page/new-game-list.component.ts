import { Component, OnDestroy, OnInit } from '@angular/core';
import { IGameListResult } from '../../../../../entities/games/models/game-list.model';
import { MENU_ITEM_LABELS } from '../../../../../shared/models/menu-items-labels.const';
import { SearchService } from '../../../../../shared/services/search.service';
import { MenuTabsService } from '../../../../../shared/services/menu-tabs.service';
import { GamesService } from '../../../../../entities/games/api/games.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-new-game-list',
  templateUrl: 'new-game-list.component.html',
  styleUrl: 'new-game-list.component.scss',
})
export class NewGameListComponent implements OnInit, OnDestroy {
  public games: IGameListResult[] = [];

  constructor(
    private readonly router: Router,
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
    this.fetchGameList();
  }

  public navigateToGameDetails(id: number): void {
    this.router.navigateByUrl(`games/${id}/details`);
  }

  public handleSearchExpression(searchExpression: string) {
    this.fetchGameList(searchExpression);
  }

  private fetchGameList(searchExpression: string = '') {
    this.gameService
      .getNewGameList(searchExpression)
      .subscribe((gameList) => (this.games = gameList ?? []));
  }
}
