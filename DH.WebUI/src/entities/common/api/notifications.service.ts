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
    return this.api.get<IUserNotification[]>('/api/notifications');
  }

  public areAnyActiveNotifications(): Observable<boolean> {
    return this.api.get<boolean>('/api/notifications/are-any-active', {
      backgroundRequest: true,
    });
  }

  public markNotificationAsViewed(id: number): Observable<null> {
    return this.api.post('/api/notifications/marked-as-viewed', id);
  }

  public markedAsViewAllUserNotification(): Observable<null> {
    return this.api.post('/api/notifications/mark-all', {});
  }

  public clearUserAllNotification(): Observable<null> {
    return this.api.post('/api/notifications/clear-all', {});
  }
}
