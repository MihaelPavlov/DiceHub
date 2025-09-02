import { Component, Injector, OnDestroy } from '@angular/core';
import { ReservationManagementNavigationComponent } from '../../../../../pages/reservation-management/page/reservation-management-navigation.component';
import { Observable } from 'rxjs';
import { SpaceManagementService } from '../../../../../entities/space-management/api/space-management.service';
import { ReservationConfirmationDialog } from '../../../dialogs/reservation-status-confirmation/reservation-confirmation.dialog';
import { MatDialog } from '@angular/material/dialog';
import { ReservationType } from '../../../enums/reservation-type.enum';
import { ReservationStatus } from '../../../../../shared/enums/reservation-status.enum';
import { IActiveReservedTable } from '../../../../../entities/space-management/models/active-reserved-table.model';
import { Router } from '@angular/router';
import { DateHelper } from '../../../../../shared/helpers/date-helper';
import { TranslateService } from '@ngx-translate/core';
import { FULL_ROUTE } from '../../../../../shared/configs/route.config';
import { LanguageService } from '../../../../../shared/services/language.service';
import { SupportLanguages } from '../../../../../entities/common/models/support-languages.enum';
import { user } from '@angular/fire/auth';

@Component({
  selector: 'app-space-table-active-reservations',
  templateUrl: 'space-table-active-reservations.component.html',
  styleUrl: 'space-table-active-reservations.component.scss',
})
export class SpaceTableActiveReservations implements OnDestroy {
  public reservedGames$!: Observable<IActiveReservedTable[]>;
  public showFilter: boolean = false;
  public expandedReservationId: number | null = null;

  public readonly ReservationStatus = ReservationStatus;
  public readonly DATE_TIME_FORMAT: string = DateHelper.DATE_TIME_FORMAT;

  private reservationNavigationRef!: ReservationManagementNavigationComponent | null;
  constructor(
    private readonly injector: Injector,
    private readonly router: Router,
    private readonly dialog: MatDialog,
    private readonly spaceManagementService: SpaceManagementService,
    private readonly translateService: TranslateService,
    private readonly languageService: LanguageService
  ) {
    this.reservationNavigationRef = this.injector.get(
      ReservationManagementNavigationComponent,
      null
    );

    if (this.reservationNavigationRef)
      this.reservationNavigationRef.header.next(
        this.translateService.instant('reservation_management.reservations')
      );
  }

  public get getCurrentLanguage(): SupportLanguages {
    return this.languageService.getCurrentLanguage();
  }

  public ngOnInit(): void {
    this.reservedGames$ =
      this.spaceManagementService.getActiveReservedTableList();
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

  public approveReservation(
    reservationDate: Date,
    numberOfGuests: number,
    phoneNumber: string,
    userLanguage: string
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
          phoneNumber,
          userLanguage,
        },
      });

      dialogRef.afterClosed().subscribe((result) => {
        if (result) {
          this.reservedGames$ =
            this.spaceManagementService.getActiveReservedTableList();
          this.expandedReservationId = null;
        }
      });
    }
  }

  public declineReservation(
    reservationDate: Date,
    numberOfGuests: number,
    phoneNumber: string,
    userLanguage: string
  ): void {
    if (this.expandedReservationId) {
      const dialogRef = this.dialog.open(ReservationConfirmationDialog, {
        width: '17rem',
        data: {
          type: ReservationType.Table,
          reservationId: this.expandedReservationId,
          status: ReservationStatus.Declined,
          reservationDate,
          phoneNumber,
          userLanguage,
          numberOfGuests,
        },
      });

      dialogRef.afterClosed().subscribe((result) => {
        if (result) {
          this.reservedGames$ =
            this.spaceManagementService.getActiveReservedTableList();
          this.expandedReservationId = null;
        }
      });
    }
  }

  public onHistory(): void {
    this.router.navigateByUrl(FULL_ROUTE.RESERVATION_MANAGEMENT.TABLE_HISTORY);
  }
}
