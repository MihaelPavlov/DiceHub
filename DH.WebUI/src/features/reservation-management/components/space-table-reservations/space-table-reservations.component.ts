import { Component, Injector, OnDestroy } from '@angular/core';
import { ReservationManagementNavigationComponent } from '../../../../pages/reservation-management/page/reservation-management-navigation.component';
import { Observable } from 'rxjs';
import { IReservedTable } from '../../../../entities/space-management/models/reserved-table.model';
import { SpaceManagementService } from '../../../../entities/space-management/api/space-management.service';
import { ToastService } from '../../../../shared/services/toast.service';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../../shared/models/toast.model';
import { ReservationConfirmationDialog } from '../../dialogs/reservation-status-confirmation/reservation-confirmation.dialog';
import { MatDialog } from '@angular/material/dialog';
import { ReservationType } from '../../enums/reservation-type.enum';
import { ReservationStatus } from '../../../../shared/enums/reservation-status.enum';

@Component({
  selector: 'app-space-table-reservations',
  templateUrl: 'space-table-reservations.component.html',
  styleUrl: 'space-table-reservations.component.scss',
})
export class SpaceTableReservations implements OnDestroy {
  private reservationNavigationRef!: ReservationManagementNavigationComponent | null;

  public reservedGames$!: Observable<IReservedTable[]>;
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
  }

  public ngOnInit(): void {
    this.reservedGames$ = this.spaceManagementService.getReservedTableList();
  }

  public ngOnDestroy(): void {
    if (this.reservationNavigationRef)
      this.reservationNavigationRef.removeActiveChildComponent();
  }

  public toggleItem(
    reservationId: number,
    reservationStatus: ReservationStatus
  ): void {
    if (reservationStatus !== ReservationStatus.None) {
      return;
    }
    this.expandedReservationId =
      this.expandedReservationId === reservationId ? null : reservationId;
  }

  public isExpanded(reservationId: number): boolean {
    return this.expandedReservationId === reservationId;
  }

  public approveReservation(
    reservationDate: Date,
    numberOfGuests: number
  ): void {
    if (this.expandedReservationId) {
      const dialogRef = this.dialog.open(ReservationConfirmationDialog, {
        width: '17rem',
        data: {
          type: ReservationType.Table,
          reservationId: this.expandedReservationId,
          status: ReservationStatus.Approved,
          reservationDate,
          numberOfGuests,
        },
      });

      dialogRef.afterClosed().subscribe((result) => {
        if (result) {
          this.reservedGames$ =
            this.spaceManagementService.getReservedTableList();
          this.expandedReservationId = null;
        }
      });
    }
  }

  public declineReservation(
    reservationDate: Date,
    numberOfGuests: number
  ): void {
    if (this.expandedReservationId) {
      const dialogRef = this.dialog.open(ReservationConfirmationDialog, {
        width: '17rem',
        data: {
          type: ReservationType.Table,
          reservationId: this.expandedReservationId,
          status: ReservationStatus.Declined,
          reservationDate,
          numberOfGuests,
        },
      });

      dialogRef.afterClosed().subscribe((result) => {
        if (result) {
          this.reservedGames$ =
            this.spaceManagementService.getReservedTableList();
          this.expandedReservationId = null;
        }
      });
    }
  }
}
