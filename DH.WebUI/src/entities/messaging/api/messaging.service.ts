import { Observable } from 'rxjs';
import { PATH } from '../../../shared/configs/path.config';
import { inject, Injectable } from '@angular/core';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { getToken } from 'firebase/messaging';
import { Messaging } from '@angular/fire/messaging';
import { environment } from '../../../app/environment';

@Injectable({
  providedIn: 'root',
})
export class MessagingService {
  private readonly _messaging: Messaging = inject(Messaging);
  private readonly _env = environment;

  constructor(private readonly api: RestApiService) {}

  public saveToken(deviceToken: string): Observable<null> {
    return this.api.post(
      `/${PATH.MESSAGING.CORE}/${PATH.MESSAGING.SAVE_TOKEN}`,
      {
        deviceToken,
      }
    );
  }

  /**
   * Retrieve the device token from Firebase Messaging
   */
  public async getDeviceToken(): Promise<void> {
    try {
      const token = await getToken(this._messaging, {
        vapidKey: this._env.firebase.vapidKey,
      });
      if (token) {
        console.log('Device token retrieved:', token);
        this.saveToken(token);
      } else {
        console.warn('No device token retrieved.');
      }
    } catch (error) {
      console.warn('Error retrieving device token', error);
    }
  }

  /**
   * Retrieve the device token from Firebase Messaging
   */
  public async getDeviceTokenForRegistration(): Promise<string> {
    try {
      const token = await getToken(this._messaging, {
        vapidKey: this._env.firebase.vapidKey,
      });
      return token;
    } catch (error) {
      console.warn('Error retrieving device token', error);
    }
    return '';
  }
}
