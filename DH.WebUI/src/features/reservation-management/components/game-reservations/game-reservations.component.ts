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
  public expandedItemId: number | null = null;
  public leftArrowKey: string = 'arrow_circle_left';
  public rightArrowKey: string = 'arrow_circle_right';

  constructor(
    private readonly injector: Injector,
    private readonly gameService: GamesService
  ) {
    this.reservationNavigationRef = this.injector.get(
      ReservationManagementNavigationComponent,
      null
    );
  }
  public toggleItem(reservationId: number): void {
    this.expandedItemId =
      this.expandedItemId === reservationId ? null : reservationId;
  }

  public isExpanded(reservationId: number): boolean {
    return this.expandedItemId === reservationId;
  }

  public ngOnInit(): void {
    this.reservedGames$ = this.gameService.getReservations();
  }

  public ngOnDestroy(): void {
    if (this.reservationNavigationRef)
      this.reservationNavigationRef.removeActiveChildComponent();
  }
}
