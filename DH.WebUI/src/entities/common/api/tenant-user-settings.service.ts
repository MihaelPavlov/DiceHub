import { BehaviorSubject, Observable } from 'rxjs';
import { AssistiveTouchComponent } from '../../../shared/components/assistive-touch/assistive-touch.component';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { Injectable } from '@angular/core';
import { AssistiveTouchSettings } from '../models/assistive-touch-settings.model';

@Injectable({
  providedIn: 'root',
})
export class TenantUserSettingsService {
  private readonly assistiveTouchSettingsSubject$: BehaviorSubject<AssistiveTouchSettings | null> =
    new BehaviorSubject<AssistiveTouchSettings | null>(null);

  public assistiveTouchSettings$ =
    this.assistiveTouchSettingsSubject$.asObservable();

  constructor(private readonly api: RestApiService) {}

  public getAssistiveTouchSettings(): Observable<AssistiveTouchSettings> {
    return this.api.get<AssistiveTouchSettings>('/tenantUserSettings/assistive-touch-settings');
  }

  public updateAssistiveTouchSettings(settings: AssistiveTouchSettings): void {
    this.api.post('/tenantUserSettings/assistive-touch-settings', { payload: settings }).subscribe({
      next: () => {
        this.getAssistiveTouchSettings().subscribe({
          next: (setting) => {
            console.log('from api settings->', setting);

            if (setting) {
              this.assistiveTouchSettingsSubject$.next(setting);
            }
          },
        });
      },
    });
  }
}
