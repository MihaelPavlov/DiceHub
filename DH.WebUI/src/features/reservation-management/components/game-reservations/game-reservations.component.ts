import { Component, Injector, OnDestroy, OnInit } from '@angular/core';
import { ReservationManagementNavigationComponent } from '../../../../pages/reservation-management/page/reservation-management-navigation.component';
import { Observable } from 'rxjs';
import { IReservedGame } from '../../../../entities/games/models/reserved-game.model';
import { GamesService } from '../../../../entities/games/api/games.service';

@Component({
  selector: 'app-game-reservations',
  templateUrl: 'game-reservations.component.html',
  styleUrl: 'game-reservations.component.scss',
})
export class GameReservations implements OnInit, OnDestroy {
  private reservationNavigationRef!: ReservationManagementNavigationComponent | null;
  public reservedGames$!: Observable<IReservedGame[]>;
  public showFilter: boolean = false;
  expandedItemId: number | null = null; // Track the currently expanded item
  leftArrowKey: string = 'arrow_circle_left';
  rightArrowKey: string = 'arrow_circle_right';
  constructor(
    private readonly injector: Injector,
    private readonly gameService: GamesService
  ) {
    this.reservationNavigationRef = this.injector.get(
      ReservationManagementNavigationComponent,
      null
    );
  }
  toggleItem(gameId: number): void {
    this.expandedItemId = this.expandedItemId === gameId ? null : gameId; // Toggle between current and none
  }

  isExpanded(gameId: number): boolean {
    return this.expandedItemId === gameId;
  }

  public ngOnInit(): void {
    this.reservedGames$ = this.gameService.getReservations();
  }

  public ngOnDestroy(): void {
    if (this.reservationNavigationRef)
      this.reservationNavigationRef?.removeActiveChildComponent();
  }

  public toggleFilter(): void {
    this.showFilter = !this.showFilter;
  }
}
