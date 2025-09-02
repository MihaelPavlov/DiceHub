import { Component, Injector, OnDestroy } from '@angular/core';
import { ReservationManagementNavigationComponent } from '../../../../../pages/reservation-management/page/reservation-management-navigation.component';
import { BehaviorSubject, Observable } from 'rxjs';
import { SpaceManagementService } from '../../../../../entities/space-management/api/space-management.service';
import { MatDialog } from '@angular/material/dialog';
import { ReservationStatus } from '../../../../../shared/enums/reservation-status.enum';
import { ReservationDetailsDialog } from '../../../dialogs/reservation-details/reservation-details.dialog';
import { ReservationDetailsActions } from '../../../dialogs/enums/reservation-details-actions.enum';
import { ReservationType } from '../../../enums/reservation-type.enum';
import { ITableReservationHistory } from '../../../../../entities/space-management/models/table-reservation-history.model';
import { DateHelper } from '../../../../../shared/helpers/date-helper';
import { ReservationConfirmationDialog } from '../../../dialogs/reservation-status-confirmation/reservation-confirmation.dialog';
import { TranslateService } from '@ngx-translate/core';
import { LanguageService } from '../../../../../shared/services/language.service';
import { SupportLanguages } from '../../../../../entities/common/models/support-languages.enum';

@Component({
  selector: 'app-space-table-reservation-history',
  templateUrl: 'space-table-reservation-history.component.html',
  styleUrl: 'space-table-reservation-history.component.scss',
})
export class SpaceTableReservationHistory implements OnDestroy {
  public reservedTables$!: Observable<ITableReservationHistory[] | null>;
  public showFilter: boolean = false;

  public expandedReservationId: BehaviorSubject<number | null> =
    new BehaviorSubject<number | null>(null);
  public selectedFilter: BehaviorSubject<ReservationStatus | null> =
    new BehaviorSubject<ReservationStatus | null>(null);

  public readonly ReservationStatus = ReservationStatus;
  public readonly DATE_TIME_FORMAT: string = DateHelper.DATE_TIME_FORMAT;

  private reservationNavigationRef!: ReservationManagementNavigationComponent | null;

  constructor(
    private readonly injector: Injector,
    private readonly dialog: MatDialog,
    private readonly spaceManagementService: SpaceManagementService,
    private readonly translateService: TranslateService,
    private readonly languageService: LanguageService
  ) {
    this.reservationNavigationRef = this.injector.get(
      ReservationManagementNavigationComponent,
      null
    );

    this.reservationNavigationRef?.header.next(
      this.translateService.instant('reservation_management.history.header')
    );
  }

  public get getCurrentLanguage():SupportLanguages{
    return this.languageService.getCurrentLanguage();
  }

  public ngOnInit(): void {
    this.reservedTables$ = this.spaceManagementService.getReservationHistory();
  }

  public updateHistory(filter: ReservationStatus | null): void {
    this.selectedFilter.next(filter);
    this.reservedTables$ =
      this.spaceManagementService.getReservationHistory(filter);
  }

  public ngOnDestroy(): void {
    if (this.reservationNavigationRef)
      this.reservationNavigationRef.removeActiveChildComponent();
  }

  public expandedReservationIdReset(): void {
    this.expandedReservationId.next(null);
  }

  public updateReservation(id: number): void {
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
          this.spaceManagementService.getReservationHistory(
            this.selectedFilter.value
          );
        this.expandedReservationIdReset();
      }
    });
  }

  public deleteReservation(id: number): void {
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
          this.spaceManagementService.getReservationHistory(
            this.selectedFilter.value
          );
        this.expandedReservationIdReset();
      }
    });
  }

  public approveReservation(record: ITableReservationHistory): void {
    const dialogRef = this.dialog.open(ReservationConfirmationDialog, {
      width: '17rem',
      data: {
        type: ReservationType.Table,
        reservationId: record.id,
        status: ReservationStatus.Approved,
        reservationDate: record.reservationDate,
        numberOfGuests: record.numberOfGuests,
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.reservedTables$ =
          this.spaceManagementService.getReservationHistory(
            this.selectedFilter.value
          );
        this.expandedReservationIdReset();
      }
    });
  }

  public declineReservation(record: ITableReservationHistory): void {
    const dialogRef = this.dialog.open(ReservationConfirmationDialog, {
      width: '17rem',
      data: {
        type: ReservationType.Table,
        reservationId: record.id,
        status: ReservationStatus.Declined,
        reservationDate: record.reservationDate,
        numberOfGuests: record.numberOfGuests,
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.reservedTables$ =
          this.spaceManagementService.getReservationHistory(
            this.selectedFilter.value
          );
        this.expandedReservationIdReset();
      }
    });
  }
}
