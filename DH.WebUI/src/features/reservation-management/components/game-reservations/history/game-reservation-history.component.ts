import { Component, OnDestroy, Injector } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { ReservationManagementNavigationComponent } from '../../../../../pages/reservation-management/page/reservation-management-navigation.component';
import { ReservationStatus } from '../../../../../shared/enums/reservation-status.enum';
import { ReservationDetailsDialog } from '../../../dialogs/reservation-details/reservation-details.dialog';
import { ReservationDetailsActions } from '../../../dialogs/enums/reservation-details-actions.enum';
import { ReservationType } from '../../../enums/reservation-type.enum';
import { GamesService } from '../../../../../entities/games/api/games.service';
import { DateHelper } from '../../../../../shared/helpers/date-helper';
import { ReservationConfirmationDialog } from '../../../dialogs/reservation-status-confirmation/reservation-confirmation.dialog';
import { IGameReservationHistory } from '../../../../../entities/games/models/game-reservation-history.model';
import { LogLevel } from '@microsoft/signalr';

@Component({
  selector: 'app-game-reservation-history',
  templateUrl: 'game-reservation-history.component.html',
  styleUrl: 'game-reservation-history.component.scss',
})
export class GameReservationHistory implements OnDestroy {
  public reservedGames$!: Observable<IGameReservationHistory[] | null>;
  public showFilter: boolean = false;
  public expandedReservationId: number | null = null;
  public leftArrowKey: string = 'arrow_circle_left';
  public rightArrowKey: string = 'arrow_circle_right';
  public selectedFilter: ReservationStatus | null = null;

  public readonly ReservationStatus = ReservationStatus;
  public readonly DATE_TIME_FORMAT: string = DateHelper.DATE_TIME_FORMAT;

  private reservationNavigationRef!: ReservationManagementNavigationComponent | null;

  constructor(
    private readonly injector: Injector,
    private readonly dialog: MatDialog,
    private readonly gameService: GamesService
  ) {
    this.reservationNavigationRef = this.injector.get(
      ReservationManagementNavigationComponent,
      null
    );

    this.reservationNavigationRef?.header.next('History');
  }

  public applyFilter(filter: ReservationStatus | null = null): void {
    this.selectedFilter = filter;
    this.expandedReservationId = null;
    this.reservedGames$ = this.gameService.getReservationHistory(filter);
  }

  public toggleItem(reservationId: number, event: MouseEvent): void {
    this.expandedReservationId =
      this.expandedReservationId === reservationId ? null : reservationId;
  }

  public isExpanded(reservationId: number): boolean {
    return this.expandedReservationId === reservationId;
  }

  public ngOnInit(): void {
    this.reservedGames$ = this.gameService.getReservationHistory();
  }

  public ngOnDestroy(): void {
    if (this.reservationNavigationRef)
      this.reservationNavigationRef.removeActiveChildComponent();
  }

  public updateReservation(id: number, event?: MouseEvent): void {
    if (event) event.stopPropagation();

    if (this.expandedReservationId) {
      const dialogRef = this.dialog.open(ReservationDetailsDialog, {
        width: '17rem',
        data: {
          reservationId: id,
          action: ReservationDetailsActions.Edit,
          type: ReservationType.Game,
        },
      });

      dialogRef.afterClosed().subscribe((result) => {
        if (result) {
          this.reservedGames$ = this.gameService.getReservationHistory(
            this.selectedFilter
          );
          this.expandedReservationId = null;
        }
      });
    }
  }

  public deleteReservation(id: number, event?: MouseEvent): void {
    if (event) event.stopPropagation();
    if (this.expandedReservationId) {
      const dialogRef = this.dialog.open(ReservationDetailsDialog, {
        width: '17rem',
        data: {
          reservationId: id,
          action: ReservationDetailsActions.Delete,
          type: ReservationType.Game,
        },
      });

      dialogRef.afterClosed().subscribe((result) => {
        if (result) {
          this.reservedGames$ = this.gameService.getReservationHistory(
            this.selectedFilter
          );
          this.expandedReservationId = null;
        }
      });
    }
  }

  public approveReservation(
    reservationDate: Date,
    numberOfGuests: number,
    gameName: string,
    tableReservationDate: Date | null,
    event?: MouseEvent
  ): void {
    if (event) event.stopPropagation();

    if (this.expandedReservationId) {
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
          console.log(result);
          this.reservedGames$ = this.gameService.getReservationHistory(
            this.selectedFilter
          );
          this.expandedReservationId = null;
        }
      });
    }
  }

  public declineReservation(
    reservationDate: Date,
    numberOfGuests: number,
    gameName: string,
    tableReservationDate: Date | null,
    event?: MouseEvent
  ): void {
    if (event) event.stopPropagation();

    if (this.expandedReservationId) {
      console.log(gameName);

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
          this.reservedGames$ = this.gameService.getReservationHistory(
            this.selectedFilter
          );
          this.expandedReservationId = null;
        }
      });
    }
  }
}
