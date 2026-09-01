import { IClubNameResult, ITenantSettings } from './../models/tenant-settings.model';
import { Injectable } from '@angular/core';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { Observable } from 'rxjs';
import { PATH } from '../../../shared/configs/path.config';
import { GetClubInfoModel } from '../../profile/models/get-club-info.interface';

@Injectable({
  providedIn: 'root',
})
export class TenantSettingsService {
  constructor(private readonly api: RestApiService) {}

  // Anonymous endpoint, but it still needs the active tenant (via the
  // X-Tenant-Id header) to return that club's name and logo rather than the
  // global default. Omitting `requiredTenant: false` lets rest-api.service
  // attach the header when a tenant context exists, while pre-tenant flows
  // (register / forgot-password with no selected club) still work without it.
  public getClubName(): Observable<IClubNameResult> {
    return this.api.get<IClubNameResult>(
      `/${PATH.TENANT_SETTINGS.CORE}/${PATH.TENANT_SETTINGS.GET_CLUB_NAME}`
    );
  }

   public getClubInfo(): Observable<GetClubInfoModel> {
    return this.api.get<GetClubInfoModel>(
      `/${PATH.TENANT_SETTINGS.CORE}/${PATH.TENANT_SETTINGS.GET_CLUB_INFO}`
    );
  }

  public get(): Observable<ITenantSettings> {
    return this.api.get<ITenantSettings>(`/${PATH.TENANT_SETTINGS.CORE}`);
  }

  public update(command: ITenantSettings): Observable<null> {
    return this.api.put(`/${PATH.TENANT_SETTINGS.CORE}`, {
      ...command,
    });
  }

  // `logo` is passed as an in-memory Blob (not the raw picked File): on Android
  // the File's backing content URI can stop being readable between picking the
  // image and submitting, which makes the multipart POST hang with no error.
  //
  // The endpoint responds with the logo URL as a bare `text/plain` string, so
  // responseType must be 'text' - otherwise HttpClient tries to JSON.parse it
  // and fails with a "200 Http failure during parsing" even though the upload
  // succeeded server-side.
  public updateLogo(logo: Blob, fileName: string): Observable<string | null> {
    const formData = new FormData();
    formData.append('logoFile', logo, fileName);

    return this.api.post<string>(
      `/${PATH.TENANT_SETTINGS.CORE}/${PATH.TENANT_SETTINGS.LOGO}`,
      formData,
      { options: { responseType: 'text' } }
    );
  }
}
