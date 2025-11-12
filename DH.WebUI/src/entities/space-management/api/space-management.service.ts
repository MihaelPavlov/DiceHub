import { Injectable } from '@angular/core';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { Observable } from 'rxjs';
import { PATH } from '../../../shared/configs/path.config';
import { IAddSpaceTableDto } from '../models/add-space-table.model';
import { ISpaceTableList } from '../models/space-table-list.model';
import { IUserActiveSpaceTableResult } from '../models/user-active-space-table.model';
import { ISpaceActivityStats } from '../models/space-activity-stats.model';
import { ISpaceTableParticipant } from '../models/table-participant.model';
import { IUpdateSpaceTableDto } from '../models/update-space-table.model';
import { ISpaceTableById } from '../models/get-space-table-by-id.model';
import { ActiveBookedTableModel } from '../models/active-booked-table.model';
import { IActiveReservedTable } from '../models/active-reserved-table.model';
import { IGetReservationById } from '../models/get-reservation-by-id.model';
import { ITableReservationHistory } from '../models/table-reservation-history.model';
import { ReservationStatus } from '../../../shared/enums/reservation-status.enum';

@Injectable({
  providedIn: 'root',
})
export class SpaceManagementService {
  constructor(private readonly api: RestApiService) {}

  public getTableById(id: number): Observable<ISpaceTableById> {
    return this.api.get<ISpaceTableById>(
      `/${PATH.SPACE_MANAGEMENT.CORE}/${id}`
    );
  }

  public getReservationById(id: number): Observable<IGetReservationById> {
    return this.api.get<IGetReservationById>(
      `/${PATH.SPACE_MANAGEMENT.CORE}/${PATH.SPACE_MANAGEMENT.GET_RESERVATION_BY_ID}/${id}`
    );
  }

  public getActiveReservedTableList(): Observable<IActiveReservedTable[]> {
    return this.api.get<IActiveReservedTable[]>(
      `/${PATH.SPACE_MANAGEMENT.CORE}/${PATH.SPACE_MANAGEMENT.GET_ACTIVE_RESERVED_TABLES}`
    );
  }

  public getActiveReservedTablesCount(): Observable<number> {
    return this.api.get<number>(
      `/${PATH.SPACE_MANAGEMENT.CORE}/${PATH.SPACE_MANAGEMENT.GET_ACTIVE_RESERVED_TABLES_COUNT}`
    );
  }

  public getActiveReservedTableList_BackgroundRequest(): Observable<
    IActiveReservedTable[]
  > {
    return this.api.get<IActiveReservedTable[]>(
      `/${PATH.SPACE_MANAGEMENT.CORE}/${PATH.SPACE_MANAGEMENT.GET_ACTIVE_RESERVED_TABLES}`,
      { backgroundRequest: true }
    );
  }

  public getReservationHistory(
    status: ReservationStatus | null = null
  ): Observable<ITableReservationHistory[] | null> {
    return this.api.post<ITableReservationHistory[]>(
      `/${PATH.SPACE_MANAGEMENT.CORE}/${PATH.SPACE_MANAGEMENT.GET_RESERVATION_HISTORY}`,
      { status }
    );
  }

  public getUserActiveTable(): Observable<IUserActiveSpaceTableResult> {
    return this.api.get<IUserActiveSpaceTableResult>(
      `/${PATH.SPACE_MANAGEMENT.CORE}/${PATH.SPACE_MANAGEMENT.GET_USER_ACTIVE_TABLE}`
    );
  }

  public getSpaceActivityStats(): Observable<ISpaceActivityStats> {
    return this.api.get<ISpaceActivityStats>(
      `/${PATH.SPACE_MANAGEMENT.CORE}/${PATH.SPACE_MANAGEMENT.GET_SPACE_ACTIVITY_STATS}`
    );
  }

  public getSpaceTableParticipantList(
    tableId: number,
    searchExpression: string = ''
  ): Observable<ISpaceTableParticipant[] | null> {
    return this.api.post<ISpaceTableParticipant[]>(
      `/${PATH.SPACE_MANAGEMENT.CORE}/${PATH.SPACE_MANAGEMENT.GET_TABLE_PARTICIPANTS}`,
      { id: tableId, searchExpression }
    );
  }

  public getSpaceAvailableTableList(
    searchExpressionName: string = ''
  ): Observable<ISpaceTableList[] | null> {
    return this.api.post<ISpaceTableList[]>(
      `/${PATH.SPACE_MANAGEMENT.CORE}/${PATH.SPACE_MANAGEMENT.GET_SPACE_AVAILABLE_TABLES}`,
      { searchExpressionName }
    );
  }

  public getActiveBookedTable(): Observable<ActiveBookedTableModel> {
    return this.api.get<ActiveBookedTableModel>(
      `/${PATH.SPACE_MANAGEMENT.CORE}/${PATH.SPACE_MANAGEMENT.GET_ACTIVE_BOOKED_TABLE}`
    );
  }

  public add(spaceTable: IAddSpaceTableDto): Observable<number | null> {
    return this.api.post<number>(`/${PATH.SPACE_MANAGEMENT.CORE}`, {
      ...spaceTable,
    });
  }

  public update(spaceTable: IUpdateSpaceTableDto): Observable<null> {
    return this.api.put(`/${PATH.SPACE_MANAGEMENT.CORE}`, {
      ...spaceTable,
    });
  }

  public updateReservation(
    id: number,
    publicNote: string,
    internalNote: string
  ): Observable<null> {
    return this.api.put(
      `/${PATH.SPACE_MANAGEMENT.CORE}/${PATH.SPACE_MANAGEMENT.UPDATE_RESERVATION}`,
      {
        id,
        publicNote,
        internalNote,
      }
    );
  }

  public approveReservation(
    reservationId: number,
    publicNote: string,
    internalNote: string
  ): Observable<null> {
    return this.api.put(
      `/${PATH.SPACE_MANAGEMENT.CORE}/${PATH.SPACE_MANAGEMENT.APPROVE_RESERVATION}`,
      {
        reservationId,
        publicNote,
        internalNote,
      }
    );
  }

  public declinedReservation(
    reservationId: number,
    publicNote: string,
    internalNote: string
  ): Observable<null> {
    return this.api.put(
      `/${PATH.SPACE_MANAGEMENT.CORE}/${PATH.SPACE_MANAGEMENT.DECLINE_RESERVATION}`,
      {
        reservationId,
        publicNote,
        internalNote,
      }
    );
  }

  public cancelReservation(reservationId: number): Observable<null> {
    return this.api.put(
      `/${PATH.SPACE_MANAGEMENT.CORE}/${PATH.SPACE_MANAGEMENT.CANCEL_RESERVATION}`,
      {
        reservationId,
      }
    );
  }

  public deleteReservation(id: number): Observable<null> {
    return this.api.delete(
      `/${PATH.SPACE_MANAGEMENT.CORE}/${PATH.SPACE_MANAGEMENT.DELETE_RESERVATION}/${id}`
    );
  }

  public join(tableId: number, password: string = ''): Observable<null> {
    return this.api.put(
      `/${PATH.SPACE_MANAGEMENT.CORE}/${PATH.SPACE_MANAGEMENT.JOIN}`,
      {
        id: tableId,
        password,
      }
    );
  }

  public bookTable(
    numberOfGuests: number,
    reservationDate: Date
  ): Observable<null> {
    return this.api.post(
      `/${PATH.SPACE_MANAGEMENT.CORE}/${PATH.SPACE_MANAGEMENT.BOOK_TABLE}`,
      {
        numberOfGuests,
        reservationDate,
      }
    );
  }

  public closeTable(tableId: number): Observable<null> {
    return this.api.post(
      `/${PATH.SPACE_MANAGEMENT.CORE}/${PATH.SPACE_MANAGEMENT.CLOSE_TABLE}`,
      {
        id: tableId,
      }
    );
  }

  public removeUserFromTable(
    tableId: number,
    userId: string
  ): Observable<null> {
    return this.api.post(
      `/${PATH.SPACE_MANAGEMENT.CORE}/${PATH.SPACE_MANAGEMENT.REMOVE_USER_FROM_TABLE}`,
      {
        id: tableId,
        userId,
      }
    );
  }

  public leaveTable(tableId: number): Observable<null> {
    return this.api.post(
      `/${PATH.SPACE_MANAGEMENT.CORE}/${PATH.SPACE_MANAGEMENT.LEAVE_TABLE}`,
      {
        id: tableId,
      }
    );
  }

  public addVirtualParticipant(spaceTableId: number): Observable<null> {
    return this.api.post(
      `/${PATH.SPACE_MANAGEMENT.CORE}/${PATH.SPACE_MANAGEMENT.ADD_VIRTUAL_PARTICIPANT}`,
      {
        spaceTableId,
      }
    );
  }

  public removeVirtualParticipant(
    spaceTableId: number,
    participantId: number
  ): Observable<null> {
    return this.api.post(
      `/${PATH.SPACE_MANAGEMENT.CORE}/${PATH.SPACE_MANAGEMENT.REMOVE_VIRTUAL_PARTICIPANT}`,
      {
        spaceTableId,
        participantId,
      }
    );
  }
}
