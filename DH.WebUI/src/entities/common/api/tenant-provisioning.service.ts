import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PATH } from '../../../shared/configs/path.config';
import { RestApiService } from '../../../shared/services/rest-api.service';

export interface ICreateTenantRequest {
  tenantId: string;
  tenantName: string;
  town: string;
  logoFileName: string;
  ownerEmail: string;
  clubPhoneNumber: string;
  starterProfile: string;
}

export interface ICreateTenantResult {
  tenantId: string;
  ownerEmail: string;
  starterProfile: string;
}

@Injectable({ providedIn: 'root' })
export class TenantProvisioningService {
  constructor(private readonly api: RestApiService) {}

  public provision(
    request: ICreateTenantRequest
  ): Observable<ICreateTenantResult | null> {
    return this.api.post<ICreateTenantResult>(
      `/${PATH.TENANT.CORE}/${PATH.TENANT.PROVISION}`,
      request,
      { requiredTenant: false }
    );
  }
}
