import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PATH } from '../../../shared/configs/path.config';
import { RestApiService } from '../../../shared/services/rest-api.service';
import {
  ICompleteTenantSetupRequest,
  ICompleteTenantSetupResult,
  ISeedGameCatalogDropdown,
  ITenantApplication,
  ITenantApplicationRequest,
  ITenantApplicationReviewRequest,
  ITenantApplicationSendEmailCodeRequest,
  ITenantApplicationVerifyEmailCodeRequest,
} from '../models/tenant-application.model';

@Injectable({
  providedIn: 'root',
})
export class TenantApplicationsService {
  constructor(private readonly api: RestApiService) {}

  public create(
    request: ITenantApplicationRequest,
    logoFile?: File | null
  ): Observable<number | null> {
    const formData = new FormData();
    formData.append('application', JSON.stringify(request));
    if (logoFile) {
      formData.append('logoFile', logoFile);
    }

    return this.api.post<number>(`/${PATH.TENANT_APPLICATIONS.CORE}`, formData);
  }

  public sendEmailVerificationCode(
    request: ITenantApplicationSendEmailCodeRequest
  ): Observable<boolean | null> {
    return this.api.post<boolean>(
      `/${PATH.TENANT_APPLICATIONS.CORE}/${PATH.TENANT_APPLICATIONS.SEND_EMAIL_VERIFICATION_CODE}`,
      request
    );
  }

  public verifyEmailCode(
    request: ITenantApplicationVerifyEmailCodeRequest
  ): Observable<boolean | null> {
    return this.api.post<boolean>(
      `/${PATH.TENANT_APPLICATIONS.CORE}/${PATH.TENANT_APPLICATIONS.VERIFY_EMAIL_CODE}`,
      request
    );
  }

  public getList(): Observable<ITenantApplication[]> {
    return this.api.get<ITenantApplication[]>(
      `/${PATH.TENANT_APPLICATIONS.CORE}`
    );
  }

  public getById(id: number): Observable<ITenantApplication> {
    return this.api.get<ITenantApplication>(
      `/${PATH.TENANT_APPLICATIONS.CORE}/${id}`
    );
  }

  public verify(
    id: number,
    request: ITenantApplicationReviewRequest
  ): Observable<unknown> {
    return this.api.post(
      `/${PATH.TENANT_APPLICATIONS.CORE}/${id}/${PATH.TENANT_APPLICATIONS.VERIFY}`,
      request
    );
  }

  public reject(
    id: number,
    request: ITenantApplicationReviewRequest
  ): Observable<unknown> {
    return this.api.post(
      `/${PATH.TENANT_APPLICATIONS.CORE}/${id}/${PATH.TENANT_APPLICATIONS.REJECT}`,
      request
    );
  }

  public getSetupSeedGames(): Observable<ISeedGameCatalogDropdown[]> {
    return this.api.get<ISeedGameCatalogDropdown[]>(
      `/${PATH.TENANT_APPLICATIONS.CORE}/${PATH.TENANT_APPLICATIONS.SETUP_SEED_GAMES}`,
      {
        requiredTenant: false,
      }
    );
  }

  public completeSetup(
    request: ICompleteTenantSetupRequest
  ): Observable<ICompleteTenantSetupResult | null> {
    return this.api.post<ICompleteTenantSetupResult>(
      `/${PATH.TENANT_APPLICATIONS.CORE}/${PATH.TENANT_APPLICATIONS.COMPLETE_SETUP}`,
      request,
      {
        requiredTenant: false,
      }
    );
  }
}
