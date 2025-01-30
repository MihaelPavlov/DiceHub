import { inject, Injectable } from '@angular/core';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { getToken } from 'firebase/messaging';
import { Messaging } from '@angular/fire/messaging';
import { environment } from '../../../app/environment';
import { AuthService } from '../../auth/auth.service';

@Injectable({
  providedIn: 'root',
})
export class MessagingService {
  private readonly _messaging: Messaging = inject(Messaging);
  private readonly _env = environment;

  constructor(
    private readonly api: RestApiService,
    private readonly authService: AuthService
  ) {}

  /**
   * Retrieve the device token from Firebase Messaging and update the token in the database
   */
  public async getDeviceToken(): Promise<void> {
    try {
      const token = await getToken(this._messaging, {
        vapidKey: this._env.firebase.vapidKey,
      });

      console.log('Device token retrieved:', token);

      if (this.authService.getUser)
        this.authService.saveToken(token).subscribe();
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
