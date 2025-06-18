import { inject, Injectable } from '@angular/core';
import { getToken } from 'firebase/messaging';
import { Messaging } from '@angular/fire/messaging';
import { environment } from '../../../app/environment';
import { AuthService } from '../../auth/auth.service';
import { Subscription } from 'rxjs';
import { FirebaseMessaging } from '@capacitor-firebase/messaging';
import { Platform } from '@angular/cdk/platform';
import { FrontEndLogService } from '../../../shared/services/frontend-log.service';
import { ToastType } from '../../../shared/models/toast.model';
import { ToastService } from '../../../shared/services/toast.service';

@Injectable({
  providedIn: 'root',
})
export class MessagingService {
  private readonly _messaging: Messaging = inject(Messaging);
  private readonly _env = environment;

  constructor(
    private readonly authService: AuthService,
    private readonly platform: Platform,
    private readonly frontEndLogService: FrontEndLogService,
    private readonly toastService: ToastService
  ) {}

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
      this.requestNotificationPermission();

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
      this.requestNotificationPermission();

      if (this.isNativePlatform()) {
        // Native mobile: use Capacitor plugin
        const permStatus = await FirebaseMessaging.requestPermissions();
        if (permStatus.receive === 'granted') {
          const tokenResult = await FirebaseMessaging.getToken();
          console.error('[FCM Native] Token:', tokenResult.token);
          return tokenResult.token;
        } else {
          console.error('[FCM Native] Permission not granted');
          return null;
        }
      } else {
        // Web browser: use Firebase JS SDK
        const registration = await navigator.serviceWorker.getRegistration(
          '/firebase-cloud-messaging-push-scope'
        );
        if (!registration) {
          console.error('No service worker registered. Registering...');
          await navigator.serviceWorker.register('/firebase-messaging-sw.js', {
            scope: '/firebase-cloud-messaging-push-scope',
          });
        }

        const token = await getToken(this._messaging, {
          vapidKey: this._env.firebase.vapidKey,
          serviceWorkerRegistration: registration!,
        });

        console.error('[FCM Web] Token:', token);
        return token;
      }
    } catch (error: any) {
      console.error('Error retrieving device token:', {
        message: error.message,
        stack: error.stack,
      });

      this.frontEndLogService.sendError(error.message, error.stack).subscribe({
        next: (response) => {
          console.log('Error logged successfully:', response);
        },
      });
      return null;
    }
  }

  public isPushUnsupportedIOS(): boolean {
    const ua = navigator.userAgent.toLowerCase();
    const isIOS = /iphone|ipad|ipod/.test(ua);
    const versionMatch = ua.match(/os (\d+)_/i); // e.g., "OS 16_3" becomes ["os 16_", "16"]
    const iosVersion = versionMatch ? parseFloat(versionMatch[1]) : 0;

    return isIOS && iosVersion < 16.4;
  }

  private isNativePlatform(): boolean {
    return !this.platform.IOS && !this.platform.ANDROID
      ? false
      : (window as any).Capacitor?.isNativePlatform?.() ?? false;
  }

  public async requestNotificationPermission(): Promise<void> {
    try {
      const permission = await Notification.requestPermission();
      if (permission === 'granted') {
        this.frontEndLogService
          .sendInfo('Notification permission granted', 'app.component.ts')
          .subscribe();
      } else {
        this.frontEndLogService
          .sendInfo(
            `Notification permission are not granted - ${permission}`,
            'app.component.ts'
          )
          .subscribe();
        this.toastService.error({
          message: `Permissions ${permission} for notifications are not granted.`,
          type: ToastType.Error,
        });
      }
    } catch (error) {
      this.frontEndLogService
        .sendError('Notification permission request error', 'app.component.ts')
        .subscribe();
    }
  }
}
