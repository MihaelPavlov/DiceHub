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
import { IReservedTable } from '../models/reserved-table.model';

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

  public getReservedTableList(): Observable<IReservedTable[]> {
    return this.api.get<IReservedTable[]>(
      `/${PATH.SPACE_MANAGEMENT.CORE}/${PATH.SPACE_MANAGEMENT.GET_RESERVED_TABLES}`
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
    reservationDate: Date,
    time: string
  ): Observable<null> {
    return this.api.post(
      `/${PATH.SPACE_MANAGEMENT.CORE}/${PATH.SPACE_MANAGEMENT.BOOK_TABLE}`,
      {
        numberOfGuests,
        reservationDate,
        time,
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
