import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { GamesService } from '../../../../../entities/games/api/games.service';
import { Observable } from 'rxjs';
import { IGameByIdResult } from '../../../../../entities/games/models/game-by-id.model';
import { MenuTabsService } from '../../../../../shared/services/menu-tabs.service';
import { NAV_ITEM_LABELS } from '../../../../../shared/models/nav-items-labels.const';
import { Location } from '@angular/common';

@Component({
  selector: 'app-game-details',
  templateUrl: 'game-details.component.html',
  styleUrl: 'game-details.component.scss',
})
export class GameDetailsComponent implements OnInit, OnDestroy {
  public game$!: Observable<IGameByIdResult>;
  private gameId!: number;

  constructor(
    private readonly gameService: GamesService,
    private readonly activeRoute: ActivatedRoute,
    private readonly menuTabsService: MenuTabsService,
    private readonly location: Location
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.GAMES);
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public ngOnInit(): void {
    this.activeRoute.params.subscribe((params: Params) => {
      this.gameId = params['id'];
      this.fetchGame();
    });
  }

  public navigateBack(): void {
    this.location.back();
  }

  public fetchGame(): void {
    this.game$ = this.gameService.getById(this.gameId);
  }
}
