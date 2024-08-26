import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { GamesService } from '../../../../../entities/games/api/games.service';
import { Observable } from 'rxjs';
import { IGameByIdResult } from '../../../../../entities/games/models/game-by-id.model';
import { MenuTabsService } from '../../../../../shared/services/menu-tabs.service';
import { NAV_ITEM_LABELS } from '../../../../../shared/models/nav-items-labels.const';
import { IGameInventory } from '../../../../../entities/games/models/game-inventory.mode';

@Component({
  selector: 'app-game-availability',
  templateUrl: 'game-availability.component.html',
  styleUrl: 'game-availability.component.scss',
})
export class GameAvailabilityComponent implements OnInit, OnDestroy {
  public game$!: Observable<IGameByIdResult>;
  public gameInventory$!: Observable<IGameInventory>;
  public availableMinutes = [1, 2, 5, 10, 15, 20, 30, 40, 50, 60];
  public currentTimer = 15;

  constructor(
    private readonly gameService: GamesService,
    private readonly activeRoute: ActivatedRoute,
    private readonly router: Router,
    private readonly menuTabsService: MenuTabsService
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.GAMES);
  }

  public ngOnInit(): void {
    this.activeRoute.params.subscribe((params: Params) => {
      const gameId = params['id'];
      this.game$ = this.gameService.getById(gameId);
      this.gameInventory$ = this.gameService.getInventory(gameId);
    });
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public navigateBackToGameList(): void {
    this.router.navigate(['games/library']);
  }

  public decreaseTimer() {
    const currentIndex = this.availableMinutes.indexOf(this.currentTimer);
    if (currentIndex > 0) {
      this.currentTimer = this.availableMinutes[currentIndex - 1];
    }
  }

  public increaseTimer() {
    const currentIndex = this.availableMinutes.indexOf(this.currentTimer);
    if (currentIndex < this.availableMinutes.length - 1) {
      this.currentTimer = this.availableMinutes[currentIndex + 1];
    }
  }

  public onReservation(gameId: number): void {
    this.gameService
      .reservation({ gameId, durationInMinutes: this.currentTimer })
      .subscribe(
        (x) => (this.gameInventory$ = this.gameService.getInventory(gameId))
      );
  }
}
