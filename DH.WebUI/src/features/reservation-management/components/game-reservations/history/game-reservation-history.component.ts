import { Component, OnDestroy, Injector } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { ReservationManagementNavigationComponent } from '../../../../../pages/reservation-management/page/reservation-management-navigation.component';
import { ReservationStatus } from '../../../../../shared/enums/reservation-status.enum';
import { ReservationDetailsDialog } from '../../../dialogs/reservation-details/reservation-details.dialog';
import { ReservationDetailsActions } from '../../../dialogs/enums/reservation-details-actions.enum';
import { ReservationType } from '../../../enums/reservation-type.enum';
import { ITableReservationHistory } from '../../../../../entities/space-management/models/table-reservation-history.model';
import { GamesService } from '../../../../../entities/games/api/games.service';

@Component({
  selector: 'app-game-reservation-history',
  templateUrl: 'game-reservation-history.component.html',
  styleUrl: 'game-reservation-history.component.scss',
})
export class GameReservationHistory implements OnDestroy {
  private reservationNavigationRef!: ReservationManagementNavigationComponent | null;

  public reservedGames$!: Observable<ITableReservationHistory[]>;
  public showFilter: boolean = false;
  public expandedReservationId: number | null = null;
  public leftArrowKey: string = 'arrow_circle_left';
  public rightArrowKey: string = 'arrow_circle_right';
  public ReservationStatus = ReservationStatus;
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

  public toggleItem(
    reservationId: number,
    reservationStatus: ReservationStatus
  ): void {
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

  public updateReservation(id: number): void {
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
          this.reservedGames$ = this.gameService.getReservationHistory();
          this.expandedReservationId = null;
        }
      });
    }
  }

  public deleteReservation(id: number): void {
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
          this.reservedGames$ = this.gameService.getReservationHistory();
          this.expandedReservationId = null;
        }
      });
    }
  }
}
