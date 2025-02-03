import { Component, Injector, OnDestroy } from '@angular/core';
import { ReservationManagementNavigationComponent } from '../../../../../pages/reservation-management/page/reservation-management-navigation.component';
import { Observable } from 'rxjs';
import { SpaceManagementService } from '../../../../../entities/space-management/api/space-management.service';
import { MatDialog } from '@angular/material/dialog';
import { ReservationStatus } from '../../../../../shared/enums/reservation-status.enum';
import { ReservationDetailsDialog } from '../../../dialogs/reservation-details/reservation-details.dialog';
import { ReservationDetailsActions } from '../../../dialogs/enums/reservation-details-actions.enum';
import { ReservationType } from '../../../enums/reservation-type.enum';
import { ITableReservationHistory } from '../../../../../entities/space-management/models/table-reservation-history.model';
import { DateHelper } from '../../../../../shared/helpers/date-helper';

@Component({
  selector: 'app-space-table-reservation-history',
  templateUrl: 'space-table-reservation-history.component.html',
  styleUrl: 'space-table-reservation-history.component.scss',
})
export class SpaceTableReservationHistory implements OnDestroy {
  public reservedTables$!: Observable<ITableReservationHistory[]>;
  public showFilter: boolean = false;
  public expandedReservationId: number | null = null;

  public readonly ReservationStatus = ReservationStatus;
  public readonly DATE_TIME_FORMAT: string = DateHelper.DATE_TIME_FORMAT;

  private reservationNavigationRef!: ReservationManagementNavigationComponent | null;

  constructor(
    private readonly injector: Injector,
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
    this.reservedTables$ = this.spaceManagementService.getReservationHistory();
  }

  public ngOnDestroy(): void {
    if (this.reservationNavigationRef)
      this.reservationNavigationRef.removeActiveChildComponent();
  }

  public toggleItem(reservationId: number): void {
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
          type: ReservationType.Table,
        },
      });

      dialogRef.afterClosed().subscribe((result) => {
        if (result) {
          this.reservedTables$ =
            this.spaceManagementService.getReservationHistory();
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
          type: ReservationType.Table,
        },
      });

      dialogRef.afterClosed().subscribe((result) => {
        if (result) {
          this.reservedTables$ =
            this.spaceManagementService.getReservationHistory();
          this.expandedReservationId = null;
        }
      });
    }
  }
}
