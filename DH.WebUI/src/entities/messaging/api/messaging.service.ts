import { inject, Injectable } from '@angular/core';
import { getToken } from 'firebase/messaging';
import { Messaging } from '@angular/fire/messaging';
import { environment } from '../../../shared/environments/environment.development';
import { AuthService } from '../../auth/auth.service';
import { Subscription } from 'rxjs';
import {
  FirebaseMessaging,
  Importance,
  Visibility,
} from '@capacitor-firebase/messaging';
import { Capacitor } from '@capacitor/core';
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

  private nativeListenersReady = false;

  /**
   * Native (Capacitor) FCM wiring. The web `onMessage` / service-worker path
   * never fires inside the Android WebView, so foreground messages, token
   * rotation and notification taps all have to go through the plugin instead.
   * Idempotent - the listeners are process-global and only registered once.
   */
  public async initNativeMessaging(handlers: {
    onForegroundMessage: () => void;
    onNotificationTap: (data?: Record<string, string>) => void;
  }): Promise<void> {
    if (!this.isNativePlatform() || this.nativeListenersReady) {
      return;
    }
    this.nativeListenersReady = true;

    await this.createDefaultChannel();

    await FirebaseMessaging.addListener('notificationReceived', (event) => {
      console.log('[FCM Native] foreground message', event.notification);
      handlers.onForegroundMessage();
    });

    await FirebaseMessaging.addListener(
      'notificationActionPerformed',
      (event) => {
        console.log('[FCM Native] notification tapped', event.notification);
        handlers.onNotificationTap(
          event.notification?.data as Record<string, string> | undefined
        );
      }
    );

    await FirebaseMessaging.addListener('tokenReceived', (event) => {
      console.log('[FCM Native] token rotated');
      this.saveTokenIfLoggedIn(event.token, 'rotation');
    });
  }

  /**
   * Native startup registration: make sure permission is granted, the Android
   * channel the backend targets exists, and the backend has this device's
   * current token. Safe to call on every app launch.
   */
  public async ensureNativeRegistration(): Promise<void> {
    if (!this.isNativePlatform()) {
      return;
    }
    try {
      const perm = await FirebaseMessaging.requestPermissions();
      if (perm.receive !== 'granted') {
        console.warn('[FCM Native] notification permission not granted');
        return;
      }
      await this.createDefaultChannel();
      const { token } = await FirebaseMessaging.getToken();
      this.saveTokenIfLoggedIn(token, 'startup');
    } catch (error: any) {
      console.error(
        '[FCM Native] ensureNativeRegistration failed',
        error?.message
      );
      this.frontEndLogService
        .sendError(
          'ensureNativeRegistration failed: ' + error?.message,
          'messaging.service.ts'
        )
        .subscribe();
    }
  }

  /**
   * Create (idempotently) the Android notification channel the backend pins
   * every message to via `AndroidNotification.ChannelId`. No-op off Android.
   */
  private async createDefaultChannel(): Promise<void> {
    if (Capacitor.getPlatform() !== 'android') {
      return;
    }
    try {
      await FirebaseMessaging.createChannel({
        id: 'default',
        name: 'General notifications',
        description: 'Reservations, events, rewards and challenge updates',
        importance: Importance.High,
        visibility: Visibility.Public,
      });
    } catch (error: any) {
      console.warn('[FCM Native] createChannel failed', error?.message);
    }
  }

  private saveTokenIfLoggedIn(
    token: string | null | undefined,
    reason: string
  ): void {
    if (!token || !this.authService.getUser) {
      return;
    }
    this.authService.saveToken(token).subscribe({
      error: (err) =>
        console.error(`[FCM Native] saveToken (${reason}) failed`, err?.message),
    });
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
    // Native asks via FirebaseMessaging.requestPermissions(); the web
    // Notification API is unreliable/absent in the Android WebView and its
    // "denied" result would otherwise pop a misleading "notifications are
    // blocked in your browser settings" toast inside the app.
    if (this.isNativePlatform() || typeof Notification === 'undefined') {
      return;
    }

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
    let helpUrl = '';
   if(environment.production){
      helpUrl = `${environment.defaultAppUrl}/instructions/notifications`;
   }else{
      helpUrl = `http://localhost:4200/instructions/notifications`;
   }

    this.toastService.error({
      message: `Notifications are <b>blocked</b> in your browser settings. Visit our instruction page.
              <a href="${helpUrl}"><strong style="text-decoration: underline;">Learn how to enable them</strong></a>.`,
      type: ToastType.Error,
      duration: 10000,
    });
  }
}
