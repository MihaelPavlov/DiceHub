import { Component, Injector, OnDestroy } from '@angular/core';
import { ReservationManagementNavigationComponent } from '../../../../../pages/reservation-management/page/reservation-management-navigation.component';
import { Observable } from 'rxjs';
import { SpaceManagementService } from '../../../../../entities/space-management/api/space-management.service';
import { ToastService } from '../../../../../shared/services/toast.service';
import { MatDialog } from '@angular/material/dialog';
import { ReservationStatus } from '../../../../../shared/enums/reservation-status.enum';
import { IConfirmedReservedTable } from '../../../../../entities/space-management/models/confirmed-reserved-table.model';
import { ReservationDetailsDialog } from '../../../dialogs/reservation-details/reservation-details.dialog';
import { ReservationDetailsActions } from '../../../dialogs/enums/reservation-details-actions.enum';

@Component({
  selector: 'app-space-table-reservation-history',
  templateUrl: 'space-table-reservation-history.component.html',
  styleUrl: 'space-table-reservation-history.component.scss',
})
export class SpaceTableReservationHistory implements OnDestroy {
  private reservationNavigationRef!: ReservationManagementNavigationComponent | null;

  public reservedGames$!: Observable<IConfirmedReservedTable[]>;
  public showFilter: boolean = false;
  public expandedReservationId: number | null = null;
  public leftArrowKey: string = 'arrow_circle_left';
  public rightArrowKey: string = 'arrow_circle_right';
  public ReservationStatus = ReservationStatus;
  constructor(
    private readonly injector: Injector,
    private readonly toastService: ToastService,
    private readonly dialog: MatDialog,
    private readonly spaceManagementService: SpaceManagementService
  ) {
    this.reservationNavigationRef = this.injector.get(
      ReservationManagementNavigationComponent,
      null
    );

    this.reservationNavigationRef?.header.next('History');
  }

  public ngOnInit(): void {
    this.reservedGames$ =
      this.spaceManagementService.getConfirmedReservedTableList();
  }

  public ngOnDestroy(): void {
    if (this.reservationNavigationRef)
      this.reservationNavigationRef.removeActiveChildComponent();
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

  public updateReservation(id: number): void {
    if (this.expandedReservationId) {
      const dialogRef = this.dialog.open(ReservationDetailsDialog, {
        width: '17rem',
        data: {
          reservationId: id,
          action: ReservationDetailsActions.Edit,
        },
      });

      dialogRef.afterClosed().subscribe((result) => {
        if (result) {
          this.reservedGames$ =
            this.spaceManagementService.getConfirmedReservedTableList();
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
        },
      });

      dialogRef.afterClosed().subscribe((result) => {
        if (result) {
          this.reservedGames$ =
            this.spaceManagementService.getConfirmedReservedTableList();
          this.expandedReservationId = null;
        }
      });
    }
  }
}
