import { Injectable } from '@angular/core';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { Observable } from 'rxjs';
import { PATH } from '../../../shared/configs/path.config';
import { IAddSpaceTableDto } from '../models/add-space-table.model';
import { ISpaceTableList } from '../models/space-table-list.model';

@Injectable({
  providedIn: 'root',
})
export class SpaceManagementService {
  constructor(private readonly api: RestApiService) {}

  public add(spaceTable: IAddSpaceTableDto): Observable<number | null> {
    return this.api.post<number>(`/${PATH.SPACE_MANAGEMENT.CORE}`, {
      ...spaceTable,
    });
  }

  public join(): Observable<null> {
    return this.api.put(`/${PATH.SPACE_MANAGEMENT.CORE}`, {});
  }

  public getList(
    searchExpressionName: string = ''
  ): Observable<ISpaceTableList[] | null> {
    return this.api.post<ISpaceTableList[]>(
      `/${PATH.SPACE_MANAGEMENT.CORE}/${PATH.SPACE_MANAGEMENT.LIST}`,
      { searchExpressionName }
    );
  }
}
