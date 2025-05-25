import { inject, Injectable } from '@angular/core';
import { getToken } from 'firebase/messaging';
import { Messaging } from '@angular/fire/messaging';
import { environment } from '../../../app/environment';
import { AuthService } from '../../auth/auth.service';
import { Subscription } from 'rxjs';
import { LoadingService } from '../../../shared/services/loading.service';

@Injectable({
  providedIn: 'root',
})
export class MessagingService {
  private readonly _messaging: Messaging = inject(Messaging);
  private readonly _env = environment;

  constructor(
    private readonly authService: AuthService
  ) {
  }

  private async registerServiceWorker(): Promise<ServiceWorkerRegistration | null> {
    if (!('serviceWorker' in navigator)) {
      console.warn('Service Workers not supported in this browser');
      return null;
    }

    try {
      const registration = await navigator.serviceWorker.register(
        '/firebase-messaging-sw.js',
        { scope: '/firebase-cloud-messaging-push-scope' }
      );

      if (!registration.active) {
        console.debug('Waiting for service worker to activate...');
        await new Promise<void>((resolve) => {
          const installingWorker =
            registration.installing || registration.waiting;
          if (installingWorker) {
            installingWorker.addEventListener('statechange', () => {
              if (installingWorker.state === 'activated') {
                resolve();
              }
            });
          } else {
            resolve();
          }
        });
      }
      console.info(
        'Service Worker registered successfully:',
        registration.scope
      );
      return registration;
    } catch (error: any) {
      console.error('Service Worker registration failed:', {
        message: error.message,
        code: error.code,
        stack: error.stack,
      });
      return null;
    }
  }

  public async getDeviceToken(): Promise<void> {
    try {
      const registration = await navigator.serviceWorker.getRegistration(
        '/firebase-cloud-messaging-push-scope'
      );
      if (registration?.active) {
        console.debug(
          'Using existing active service worker:',
          registration.scope
        );
      } else {
        console.warn('No active service worker. Registering...');
        await this.registerServiceWorker();
      }

      const token = await getToken(this._messaging, {
        vapidKey: this._env.firebase.vapidKey,
        serviceWorkerRegistration: registration,
      });

      const user = this.authService.getUser;
      console.info(
        `Device token retrieved for user ${user?.id || 'unknown'}:`,
        token
      );

      if (user) {
        console.debug('Subscribing to save token for user:', user.id);
        const subscription: Subscription = this.authService
          .saveToken(token)
          .subscribe({
            error: (err) =>
              console.error('Failed to save token:', {
                message: err.message,
                code: err.code,
                stack: err.stack,
              }),
            complete: () => {
              console.debug('Token saved successfully');
              subscription.unsubscribe();
            },
          });
      } 
    } catch (error: any) {
      console.warn('Error retrieving device token:', {
        message: error.message,
        code: error.code,
        stack: error.stack,
      });
    }
  }

  public async getDeviceTokenForRegistration(): Promise<string | null> {
    try {
      // this.loadingService.loadingOn();
      const registration = await navigator.serviceWorker.getRegistration(
        '/firebase-cloud-messaging-push-scope'
      );
      if (registration?.active) {
        console.debug(
          'Using existing active service worker:',
          registration.scope
        );
      } else {
        console.warn('No active service worker. Registering...');
        await this.registerServiceWorker();
      }

      const token = await getToken(this._messaging, {
        vapidKey: this._env.firebase.vapidKey,
        serviceWorkerRegistration: registration,
      });
      console.info('Device token retrieved:', token);
      // this.loadingService.loadingOff();
      return token;
    } catch (error: any) {
      console.warn('Error retrieving device token:', {
        message: error.message,
        code: error.code,
        stack: error.stack,
      });
      // this.loadingService.loadingOff();
      return null;
    }
  }
}
