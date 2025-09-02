import { DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SpaceManagementService } from '../../../entities/space-management/api/space-management.service';
import { combineLatest, throwError } from 'rxjs';
import { IUserActiveSpaceTableResult } from '../../../entities/space-management/models/user-active-space-table.model';
import { ISpaceActivityStats } from '../../../entities/space-management/models/space-activity-stats.model';
import {
  ActiveBookedTableModel,
  getKeyFriendlyNames,
} from '../../../entities/space-management/models/active-booked-table.model';
import { MatDialog } from '@angular/material/dialog';
import { QrCodeType } from '../../../entities/qr-code-scanner/enums/qr-code-type.enum';
import { AuthService } from '../../../entities/auth/auth.service';
import { ReservationQrCodeDialog } from '../../../shared/dialogs/reservation-qr-code/reservation-qr-code.component';
import { ReservationStatus } from '../../../shared/enums/reservation-status.enum';
import { GamesService } from '../../../entities/games/api/games.service';
import { ActiveReservedGame } from '../../../entities/games/models/active-reserved-game.model';
import { DateHelper } from '../../../shared/helpers/date-helper';
import { FULL_ROUTE, ROUTE } from '../../../shared/configs/route.config';
import { LanguageService } from '../../../shared/services/language.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-club-space-management',
  templateUrl: 'club-space-management.component.html',
  styleUrl: 'club-space-management.component.scss',
})
export class ClubSpaceManagementComponent implements OnInit {
  public userActiveTableInfo!: IUserActiveSpaceTableResult;
  public spaceActivityStats!: ISpaceActivityStats;
  public activeBookedTableModel: ActiveBookedTableModel | null = null;
  public activeGameReservationModel: ActiveReservedGame | null = null;

  public ReservationStatus = ReservationStatus;

  private hostMessages = [
    'space_management.table_messages.host.live',
    'space_management.table_messages.host.in_progress',
    'space_management.table_messages.host.rolled_dice',
    'space_management.table_messages.host.hot_table',
    'space_management.table_messages.host.running_show',
  ];

  private participantMessages = [
    'space_management.table_messages.participant.in_game',
    'space_management.table_messages.participant.seat_taken',
    'space_management.table_messages.participant.joined_table',
    'space_management.table_messages.participant.part_of_crew',
    'space_management.table_messages.participant.table_vibes',
  ];

  public randomHostMessage: string | null = null;
  public randomParticipantMessage: string | null = null;

  constructor(
    private readonly router: Router,
    private readonly spaceManagementService: SpaceManagementService,
    public readonly gameService: GamesService,
    private readonly dialog: MatDialog,
    private readonly authService: AuthService,
    private readonly datePipe: DatePipe,
    private readonly languageService: LanguageService,
    private readonly translateService: TranslateService
  ) {}

  public ngOnInit(): void {
    combineLatest([
      this.spaceManagementService.getUserActiveTable(),
      this.spaceManagementService.getSpaceActivityStats(),
    ]).subscribe({
      next: ([userActiveTableResult, spaceActivityStats]) => {
        if (userActiveTableResult)
          this.userActiveTableInfo = userActiveTableResult;

        if (spaceActivityStats) this.spaceActivityStats = spaceActivityStats;

        if (this.userActiveTableInfo?.isPlayerHaveActiveTable) {
          this.randomHostMessage = this.getRandom(this.hostMessages);
        }

        if (this.userActiveTableInfo?.isPlayerParticipateInTable) {
          this.randomParticipantMessage = this.getRandom(
            this.participantMessages
          );
        }
      },
      error: (errors) => {
        throwError(() => errors);
      },
    });
    this.spaceManagementService.getActiveBookedTable().subscribe({
      next: (activeTableReservation) => {
        this.activeBookedTableModel = activeTableReservation;
      },
    });
    this.gameService.getActiveReservation().subscribe({
      next: (activeGameReservationModel) => {
        this.activeGameReservationModel = activeGameReservationModel;
      },
    });
  }

  public openDialog(): void {
    const isSameReservationDate = this.isSameDate(
      this.activeGameReservationModel?.reservationDate,
      this.activeBookedTableModel?.reservationDate
    );

    const dialogData = this.getDialogData(isSameReservationDate);

    this.dialog.open(ReservationQrCodeDialog, {
      width: '17rem',
      data: dialogData,
    });
  }

  private isSameDate(date1?: Date, date2?: Date): boolean {
    if (!date1 || !date2) return false;

    const date1Parsed = new Date(date1);
    const date2Parsed = new Date(date2);
    return (
      date1Parsed.getFullYear() === date2Parsed.getFullYear() &&
      date1Parsed.getMonth() === date2Parsed.getMonth() &&
      date1Parsed.getDate() === date2Parsed.getDate()
    );
  }

  private getDialogData(isSameReservationDate: boolean): any {
    if (
      this.activeGameReservationModel?.status === ReservationStatus.Approved &&
      this.activeBookedTableModel?.status === ReservationStatus.Approved &&
      isSameReservationDate
    ) {
      return {
        Id: this.activeGameReservationModel.id,
        Name: 'GameReservation',
        Type: QrCodeType.GameReservation,
        AdditionalData: {
          userId: this.authService.getUser?.id,
        },
      };
    }

    return {
      Id: this.activeBookedTableModel?.id,
      Name: 'TableReservation',
      Type: QrCodeType.TableReservation,
      AdditionalData: {
        userId: this.authService.getUser?.id,
      },
    };
  }

  public navigateSpaceTableList(): void {
    this.router.navigateByUrl(FULL_ROUTE.SPACE_MANAGEMENT.LIST);
  }

  public navigateSpaceTableBooking(): void {
    this.router.navigateByUrl(FULL_ROUTE.SPACE_MANAGEMENT.BOOKING);
  }

  public navigateToCreateTable(): void {
    this.router.navigateByUrl(ROUTE.QR_CODE_SCANNER);
  }

  public navigateToSpaceClubDetails(id: number | null | undefined): void {
    if (id)
      this.router.navigateByUrl(FULL_ROUTE.SPACE_MANAGEMENT.ROOM_DETAILS(id));
  }

  public getKeyValuePair(): { key: string; value: any }[] {
    if (this.activeBookedTableModel) {
      const keyFriendlyNames = getKeyFriendlyNames(this.translateService);
      const keyTransformations = this.getKeyTransformations();

      return Object.entries(this.activeBookedTableModel)
        .filter(([key]) => keyFriendlyNames[key] && keyTransformations[key])
        .map(([key, value]) => {
          const friendlyName = keyFriendlyNames[key];
          const transformedValue = keyTransformations[key]?.(value) ?? value;

          return { key: friendlyName, value: transformedValue };
        });
    }
    return [];
  }

  private getKeyTransformations(): Record<string, (value: any) => any> {
    return {
      numberOfGuests: (value) => value,
      reservationDate: (value) => {
        return this.datePipe.transform(
          new Date(value),
          DateHelper.DATE_TIME_FORMAT,
          undefined,
          this.languageService.getCurrentLanguage()
        );
      },
      isConfirmed: (value) => value,
    };
  }

  private getRandom(messages: string[]): string {
    return this.translateService.instant(
      messages[Math.floor(Math.random() * messages.length)]
    );
  }
}
