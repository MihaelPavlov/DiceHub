import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GamesService } from '../../../entities/games/api/games.service';
import { IGameListResult } from '../../../entities/games/models/game-list.model';

@Component({
  selector: 'app-games-library',
  templateUrl: 'games-library.component.html',
  styleUrl: 'games-library.component.scss',
})
export class GamesLibraryComponent implements OnInit {
  public games: IGameListResult[] = [];

  constructor(
    private readonly router: Router,
    private readonly gameService: GamesService
  ) {}

  public ngOnInit(): void {
    this.fetchGameList();
  }

  public navigateToGameDetails(id: number): void {
    this.router.navigateByUrl(`games/${id}/details`);
  }

  public handleSeachExpression(searchExpression: string) {
    this.fetchGameList(searchExpression);
  }

  private fetchGameList(searchExpression: string = '') {
    this.gameService
      .getList(searchExpression)
      .subscribe((gameList) => (this.games = gameList ? gameList : []));
  }
}
