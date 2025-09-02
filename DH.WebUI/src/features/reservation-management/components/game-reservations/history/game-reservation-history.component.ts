import { IGameReservationHistory } from './../../../../../entities/games/models/game-reservation-history.model';
import { Component, OnDestroy, Injector } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { BehaviorSubject, Observable } from 'rxjs';
import { ReservationManagementNavigationComponent } from '../../../../../pages/reservation-management/page/reservation-management-navigation.component';
import { ReservationStatus } from '../../../../../shared/enums/reservation-status.enum';
import { ReservationDetailsDialog } from '../../../dialogs/reservation-details/reservation-details.dialog';
import { ReservationDetailsActions } from '../../../dialogs/enums/reservation-details-actions.enum';
import { ReservationType } from '../../../enums/reservation-type.enum';
import { GamesService } from '../../../../../entities/games/api/games.service';
import { DateHelper } from '../../../../../shared/helpers/date-helper';
import { ReservationConfirmationDialog } from '../../../dialogs/reservation-status-confirmation/reservation-confirmation.dialog';
import { LanguageService } from '../../../../../shared/services/language.service';
import { TranslateService } from '@ngx-translate/core';
import { SupportLanguages } from '../../../../../entities/common/models/support-languages.enum';

@Component({
  selector: 'app-game-reservation-history',
  templateUrl: 'game-reservation-history.component.html',
  styleUrl: 'game-reservation-history.component.scss',
})
export class GameReservationHistory implements OnDestroy {
  public reservedGames$!: Observable<IGameReservationHistory[] | null>;
  public showFilter: boolean = false;

  public expandedReservationId: BehaviorSubject<number | null> =
    new BehaviorSubject<number | null>(null);
  public selectedFilter: BehaviorSubject<ReservationStatus | null> =
    new BehaviorSubject<ReservationStatus | null>(null);

  public readonly ReservationType = ReservationType;
  public readonly ReservationStatus = ReservationStatus;
  public readonly DATE_TIME_FORMAT: string = DateHelper.DATE_TIME_FORMAT;

  private reservationNavigationRef!: ReservationManagementNavigationComponent | null;

  constructor(
    private readonly injector: Injector,
    private readonly dialog: MatDialog,
    private readonly gameService: GamesService,
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

  public get getCurrentLanguage(): SupportLanguages {
    return this.languageService.getCurrentLanguage();
  }

  public updateHistory(filter: ReservationStatus | null): void {
    this.selectedFilter.next(filter);
    this.reservedGames$ = this.gameService.getReservationHistory(filter);
  }

  public expandedReservationIdReset(): void {
    this.expandedReservationId.next(null);
  }

  public ngOnInit(): void {
    this.reservedGames$ = this.gameService.getReservationHistory();
  }

  public ngOnDestroy(): void {
    if (this.reservationNavigationRef)
      this.reservationNavigationRef.removeActiveChildComponent();
  }

  public updateReservation(id: number): void {
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
        type: ReservationType.Game,
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.reservedGames$ = this.gameService.getReservationHistory(
          this.selectedFilter.value
        );
        this.expandedReservationIdReset();
      }
    });
  }

  public approveReservation(record: IGameReservationHistory): void {
    const dialogRef = this.dialog.open(ReservationConfirmationDialog, {
      width: '17rem',
      data: {
        type: ReservationType.Game,
        reservationId: record.id,
        status: ReservationStatus.Approved,
        reservationDate: record.reservationDate,
        numberOfGuests: record.numberOfGuests,
        gameName: record.gameName,
        tableReservationDate: record.tableReservationTime,
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.reservedGames$ = this.gameService.getReservationHistory(
          this.selectedFilter.value
        );
        this.expandedReservationIdReset();
      }
    });
  }

  public declineReservation(record: IGameReservationHistory): void {
    const dialogRef = this.dialog.open(ReservationConfirmationDialog, {
      width: '17rem',
      data: {
        type: ReservationType.Game,
        reservationId: record.id,
        status: ReservationStatus.Declined,
        reservationDate: record.reservationDate,
        numberOfGuests: record.numberOfGuests,
        gameName: record.gameName,
        tableReservationDate: record.tableReservationTime,
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.reservedGames$ = this.gameService.getReservationHistory(
          this.selectedFilter.value
        );
        this.expandedReservationIdReset();
      }
    });
  }
}
