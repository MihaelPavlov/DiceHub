import { ITenantSettings } from './../models/tenant-settings.model';
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

  public getClubName(): Observable<string> {
    return this.api.get<string>(
      `/${PATH.TENANT_SETTINGS.CORE}/${PATH.TENANT_SETTINGS.GET_CLUB_NAME}`,
      {
        requiredTenant: false,
        options: {
          responseType: 'text',
        },
      }
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

  public getLogo(): Observable<string | null> {
    return this.api.get<string | null>(
      `/${PATH.TENANT_SETTINGS.CORE}/${PATH.TENANT_SETTINGS.LOGO}`
    );
  }

  public updateLogo(logoFile: File): Observable<string | null> {
    const formData = new FormData();
    formData.append('logoFile', logoFile);

    return this.api.post<string>(
      `/${PATH.TENANT_SETTINGS.CORE}/${PATH.TENANT_SETTINGS.LOGO}`,
      formData
    );
  }
}
