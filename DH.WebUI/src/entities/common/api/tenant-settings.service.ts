import { ITenantSettings } from './../models/tenant-settings.model';
import { Injectable } from '@angular/core';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { Observable } from 'rxjs';
import { PATH } from '../../../shared/configs/path.config';

@Injectable({
  providedIn: 'root',
})
export class TenantSettingsService {
  constructor(private readonly api: RestApiService) {}

  public getClubName(): Observable<string> {
    return this.api.get<string>(
      `/${PATH.TENANT_SETTINGS.CORE}/${PATH.TENANT_SETTINGS.GET_CLUB_NAME}`,
      {
        options: {
          responseType: 'text',
        },
      }
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
}
