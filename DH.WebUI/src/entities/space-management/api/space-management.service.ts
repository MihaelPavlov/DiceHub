import { Injectable } from '@angular/core';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { Observable } from 'rxjs';
import { PATH } from '../../../shared/configs/path.config';
import { IAddSpaceTableDto } from '../models/add-space-table.model';
import { ISpaceTableList } from '../models/space-table-list.model';
import { IUserActiveSpaceTableResult } from '../models/user-active-space-table.model';
import { ISpaceActivityStats } from '../models/space-activity-stats.model';

@Injectable({
  providedIn: 'root',
})
export class SpaceManagementService {
  constructor(private readonly api: RestApiService) {}

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

  public getList(
    searchExpressionName: string = ''
  ): Observable<ISpaceTableList[] | null> {
    return this.api.post<ISpaceTableList[]>(
      `/${PATH.SPACE_MANAGEMENT.CORE}/${PATH.SPACE_MANAGEMENT.LIST}`,
      { searchExpressionName }
    );
  }

  public add(spaceTable: IAddSpaceTableDto): Observable<number | null> {
    return this.api.post<number>(`/${PATH.SPACE_MANAGEMENT.CORE}`, {
      ...spaceTable,
    });
  }

  public join(): Observable<null> {
    return this.api.put(`/${PATH.SPACE_MANAGEMENT.CORE}`, {});
  }
}
