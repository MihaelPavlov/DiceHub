import { Router } from '@angular/router';
import { Component, Injector, OnDestroy, OnInit } from '@angular/core';
import { Observable } from 'rxjs';

import { MatDialog } from '@angular/material/dialog';
import { GamesService } from '../../../../../entities/games/api/games.service';
import { ActiveReservedGame } from '../../../../../entities/games/models/active-reserved-game.model';
import { IReservedGame } from '../../../../../entities/games/models/reserved-game.model';
import { ReservationManagementNavigationComponent } from '../../../../../pages/reservation-management/page/reservation-management-navigation.component';
import { ReservationStatus } from '../../../../../shared/enums/reservation-status.enum';
import { ReservationConfirmationDialog } from '../../../dialogs/reservation-status-confirmation/reservation-confirmation.dialog';
import { ReservationType } from '../../../enums/reservation-type.enum';

@Component({
  selector: 'app-game-reservations',
  templateUrl: 'game-reservations.component.html',
  styleUrl: 'game-reservations.component.scss',
})
export class GameReservations implements OnInit, OnDestroy {
  private reservationNavigationRef!: ReservationManagementNavigationComponent | null;

  public reservedGames$!: Observable<IReservedGame[]>;
  public showFilter: boolean = false;
  public expandedReservationId: number | null = null;
  public leftArrowKey: string = 'arrow_circle_left';
  public rightArrowKey: string = 'arrow_circle_right';
  public activeReservations$!: Observable<ActiveReservedGame[]>;
  constructor(
    private readonly injector: Injector,
    private readonly gameService: GamesService,
    private readonly router: Router,
    private readonly dialog: MatDialog
  ) {
    this.reservationNavigationRef = this.injector.get(
      ReservationManagementNavigationComponent,
      null
    );

    if (this.reservationNavigationRef)
      this.reservationNavigationRef.header.next('Reservations');

    this.activeReservations$ = this.gameService.getActiveReservations();
  }
  public toggleItem(reservationId: number): void {
    this.expandedReservationId =
      this.expandedReservationId === reservationId ? null : reservationId;
  }

  public isExpanded(reservationId: number): boolean {
    return this.expandedReservationId === reservationId;
  }

  public ngOnInit(): void {
    this.reservedGames$ = this.gameService.getReservations();
  }

  public ngOnDestroy(): void {
    if (this.reservationNavigationRef)
      this.reservationNavigationRef.removeActiveChildComponent();
  }

  public onHistory(): void {
    this.router.navigateByUrl('reservations/games/history');
  }

  public approveReservation(
    reservationDate: Date,
    numberOfGuests: number,
    gameName: string,
    tableReservationDate: Date | null
  ): void {
    if (this.expandedReservationId) {
      console.log(this.expandedReservationId);

      const dialogRef = this.dialog.open(ReservationConfirmationDialog, {
        width: '17rem',
        data: {
          type: ReservationType.Game,
          reservationId: this.expandedReservationId,
          status: ReservationStatus.Approved,
          reservationDate,
          numberOfGuests,
          gameName,
          tableReservationDate,
        },
      });

      dialogRef.afterClosed().subscribe((result) => {
        if (result) {
          this.activeReservations$ = this.gameService.getActiveReservations();
          this.expandedReservationId = null;
        }
      });
    }
  }

  public declineReservation(
    reservationDate: Date,
    numberOfGuests: number,
    gameName: string,
    tableReservationDate: Date | null | undefined
  ): void {
    if (this.expandedReservationId) {
      const dialogRef = this.dialog.open(ReservationConfirmationDialog, {
        width: '17rem',
        data: {
          type: ReservationType.Game,
          reservationId: this.expandedReservationId,
          status: ReservationStatus.Declined,
          reservationDate,
          numberOfGuests,
          gameName,
          tableReservationDate,
        },
      });

      dialogRef.afterClosed().subscribe((result) => {
        if (result) {
          this.activeReservations$ = this.gameService.getActiveReservations();
          this.expandedReservationId = null;
        }
      });
    }
  }
}
