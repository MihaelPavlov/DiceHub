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
        await this.requestNotificationPermission();
     
      let registration = await navigator.serviceWorker.getRegistration(
        '/firebase-cloud-messaging-push-scope'
      );
      if (registration?.active) {
        console.debug(
          'Using existing active service worker:',
          registration.scope
        );
      } else {
        console.warn('No active service worker. Registering...');
        let newRegistration = await this.registerServiceWorker();

        if (newRegistration) registration = newRegistration;
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
      await this.requestNotificationPermission();

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

        console.info('[FCM Web] Token:', token);
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

  // 90% I think is working, but need to test on iOS 16.4 and higher
  public isPushUnsupportedIOS(): boolean {
    const ua = navigator.userAgent.toLowerCase();
    console.log('User Agent:', ua);

    const isIOS = /iphone|ipad|ipod/.test(ua);
    if (!isIOS) return false;

    // Match major and minor version: e.g., "os 16_3" -> ["os 16_3", "16", "3"]
    const versionMatch = ua.match(/os (\d+)_?(\d+)?/i);
    if (!versionMatch) return false;

    const major = parseInt(versionMatch[1], 10);
    const minor = versionMatch[2] ? parseInt(versionMatch[2], 10) : 0;
    const iosVersion = major + minor / 10;

    console.log('Is iOS:', isIOS, 'iOS Version:', iosVersion);

    return iosVersion < 16.4;
  }

  public getIOSVersion(): number {
    const ua = navigator.userAgent.toLowerCase();
    console.log('User Agent:', ua);

    const isIOS = /iphone|ipad|ipod/.test(ua);
    if (!isIOS) return 0;

    // Match major and minor version: e.g., "os 16_3" -> ["os 16_3", "16", "3"]
    const versionMatch = ua.match(/os (\d+)_?(\d+)?/i);
    if (!versionMatch) return 0;

    const major = parseInt(versionMatch[1], 10);
    const minor = versionMatch[2] ? parseInt(versionMatch[2], 10) : 0;
    const iosVersion = major + minor / 10;

    console.log('Is iOS:', isIOS, 'iOS Version:', iosVersion);

    return iosVersion;
  }

  public getNativePlatform(): string {
    if (!this.platform.IOS && !this.platform.ANDROID) {
      return 'not native platform';
    }

    const isNative = (window as any)?.Capacitor?.isNativePlatform?.();

    if (isNative === undefined) {
      return 'unknown native status';
    }

    return isNative ? 'native platform' : 'not native platform';
  }
  private isNativePlatform(): boolean {
    return !this.platform.IOS && !this.platform.ANDROID
      ? false
      : (window as any).Capacitor?.isNativePlatform?.() ?? false;
  }

  public async requestNotificationPermission(): Promise<void> {
    try {
      if (Notification.permission === 'granted') {
        this.frontEndLogService
          .sendInfo(
            'Notification permission already granted',
            'app.component.ts'
          )
          .subscribe();
        return;
      }

      const permission = await Notification.requestPermission();

      if (permission === 'granted') {
        this.frontEndLogService
          .sendInfo('Notification permission granted', 'app.component.ts')
          .subscribe();
      } else if (permission === 'denied') {
        // Notifications are blocked
        this.frontEndLogService
          .sendInfo(
            'Notification permission denied (blocked)',
            'app.component.ts'
          )
          .subscribe();

        this.showNotificationBlockedMessage();
      } else {
        // Permission was dismissed (default)
        this.frontEndLogService
          .sendInfo(
            'Notification permission dismissed (default)',
            'app.component.ts'
          )
          .subscribe();

        this.toastService.error({
          message: `You dismissed the notification permission request.`,
          type: ToastType.Error,
        });
      }
    } catch (error) {
      this.frontEndLogService
        .sendError('Notification permission request error', 'app.component.ts')
        .subscribe();
    }
  }

  private showNotificationBlockedMessage(): void {
    const helpUrl = 'https://support.google.com/chrome/answer/3220216'; // or your own help page

    this.toastService.error({
      message: `Notifications are <b>blocked</b> in your browser settings. 
              <a href="${helpUrl}" target="_blank"><strong style="text-decoration: underline;">Learn how to enable them</strong></a>.`,
      type: ToastType.Error,
      duration: 10000,
    });
  }
}
