import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GamesService } from '../../../entities/games/api/games.service';
import { IGameListResult } from '../../../entities/games/models/game-list.model';
import { MenuTabsService } from '../../../shared/services/menu-tabs.service';
import { MENU_ITEM_LABELS } from '../../../shared/models/menu-items-labels.const';

@Component({
  selector: 'app-games-library',
  templateUrl: 'games-library.component.html',
  styleUrl: 'games-library.component.scss',
})
export class GamesLibraryComponent implements OnInit, OnDestroy {
  public games: IGameListResult[] = [];

  constructor(
    private readonly router: Router,
    private readonly gameService: GamesService,
    private readonly menuTabsService: MenuTabsService
  ) {
    this.menuTabsService.setActive(MENU_ITEM_LABELS.GAMES);
  }
  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
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
      .getList(searchExpression)
      .subscribe((gameList) => (this.games = gameList ? gameList : []));
  }
}
