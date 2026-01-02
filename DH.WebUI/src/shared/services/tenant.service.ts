import { catchError, map, Observable, of } from 'rxjs';
import { ITenantListResult } from '../../entities/common/models/tenant-list.model';
import { PATH } from '../configs/path.config';
import { RestApiService } from './rest-api.service';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class TenantService {
  constructor(private readonly api: RestApiService) {}

  public getList(): Observable<ITenantListResult[] | null> {
    return this.api.get<ITenantListResult[]>(
      `/${PATH.TENANT.CORE}/${PATH.TENANT.LIST}`,
      {
        requiredTenant: false,
      }
    );
  }

  public validateTenant(tenantId: string): Observable<boolean> {
    return this.api
      .get<boolean>(`/${PATH.TENANT.CORE}/${tenantId}/${PATH.TENANT.EXISTS}`)
      .pipe(
        map(() => true),
        catchError(() => of(false))
      );
  }
}
