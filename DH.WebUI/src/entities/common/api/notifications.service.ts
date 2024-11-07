import { Injectable } from '@angular/core';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { IUserNotification } from '../models/user-notification-model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class NotificationsService {
  constructor(private readonly api: RestApiService) {}

  public getUserNotificationList(): Observable<IUserNotification[]> {
    return this.api.get<IUserNotification[]>('/notifications');
  }

  public areAnyActiveNotifications(): Observable<boolean> {
    return this.api.get<boolean>('/notifications/are-any-active');
  }

  public markNotificationAsViewed(id: number): Observable<null> {
    return this.api.post('/notifications/marked-as-viewed', id);
  }
}
