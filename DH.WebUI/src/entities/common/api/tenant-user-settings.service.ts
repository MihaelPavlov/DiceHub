import { BehaviorSubject, Observable } from 'rxjs';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { Injectable } from '@angular/core';
import { AssistiveTouchSettings } from '../models/assistive-touch-settings.model';
import { PATH } from '../../../shared/configs/path.config';
import { IUserSettings } from '../models/user-settings.model';

@Injectable({
  providedIn: 'root',
})
export class TenantUserSettingsService {
  private readonly assistiveTouchSettingsSubject$: BehaviorSubject<AssistiveTouchSettings | null> =
    new BehaviorSubject<AssistiveTouchSettings | null>(null);

  public assistiveTouchSettings$ =
    this.assistiveTouchSettingsSubject$.asObservable();

  constructor(private readonly api: RestApiService) {}

  public get(): Observable<IUserSettings> {
    return this.api.get<IUserSettings>(`/${PATH.USER_SETTINGS.CORE}`);
  }

  public update(command: IUserSettings): Observable<null> {
    return this.api.put(`/${PATH.USER_SETTINGS.CORE}`, {
      ...command,
    });
  }

  public getAssistiveTouchSettings(): Observable<AssistiveTouchSettings> {
    return this.api.get<AssistiveTouchSettings>(
      `/${PATH.USER_SETTINGS.CORE}/${PATH.USER_SETTINGS.ASSISTIVE_TOUCH_SETTINGS}`,
      { backgroundRequest: true }
    );
  }

  public updateAssistiveTouchSettings(settings: AssistiveTouchSettings): void {
    this.api
      .post(
        `/${PATH.USER_SETTINGS.CORE}/${PATH.USER_SETTINGS.ASSISTIVE_TOUCH_SETTINGS}`,
        { payload: settings },
        { backgroundRequest: true }
      )
      .subscribe({
        next: () => {
          this.getAssistiveTouchSettings().subscribe({
            next: (setting) => {
              if (setting) {
                this.assistiveTouchSettingsSubject$.next(setting);
              }
            },
          });
        },
      });
  }
}
