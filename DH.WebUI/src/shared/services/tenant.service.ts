import { Observable } from 'rxjs';
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
}
