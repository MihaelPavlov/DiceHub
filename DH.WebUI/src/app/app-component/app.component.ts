import { BehaviorSubject, Observable } from 'rxjs';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { AuthService } from '../../entities/auth/auth.service';
import { environment } from '../environment';
import { onMessage } from 'firebase/messaging';
import { Messaging } from '@angular/fire/messaging';
import { MessagingService } from '../../entities/messaging/api/messaging.service';
import { IUserInfo } from '../../entities/auth/models/user-info.model';
import { NotificationsService } from '../../entities/common/api/notifications.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent implements OnInit {
  title = 'DH.WebUI';
  private readonly _env = environment;
  public readonly userInfo: Observable<IUserInfo | null> =
    this.authService.userInfo$;
  public areAnyActiveNotificationSubject: BehaviorSubject<boolean> =
    new BehaviorSubject<boolean>(false);

  constructor(
    private readonly authService: AuthService,
    private readonly _messaging: Messaging,
    private readonly messagingService: MessagingService,
    private readonly notificationService: NotificationsService,
    private readonly cd: ChangeDetectorRef
  ) {
    // TODO: Do i need initialize the user
    this._initializeUser();
  }

  // TODO: Check this tread https://chatgpt.com/c/671602c4-266c-800d-8177-2e9b398333ba
  public ngOnInit(): void {
    this._initializeFCM();
  }

  /**
   * Initialize the user on component load
   */
  private _initializeUser(): void {
    if (!this.authService.getUser) {
      this.authService.userinfo();
    }
  }

  /**
   * Initialize Firebase Cloud Messaging related tasks
   */
  private _initializeFCM(): void {
    console.log('Initializing Firebase Cloud Messaging...');
    this._requestNotificationPermission();
    this.messagingService.getDeviceToken();
    this._listenForMessages();
  }

  public onUpdateUserNotifications() {
    this.notificationService.areAnyActiveNotifications().subscribe({
      next: (areAnyActive) => {
        this.areAnyActiveNotificationSubject.next(areAnyActive);
      },
    });
  }
  /**
   * Listen for foreground messages from Firebase Messaging
   */
  private _listenForMessages(): void {
    onMessage(this._messaging, {
      next: () => {
        this.notificationService.areAnyActiveNotifications().subscribe({
          next: (result) => {
            this.areAnyActiveNotificationSubject.next(result);
            this.cd.detectChanges();
          },
        });
      },
      error: (error) => {},
      complete: () => {
        console.log('Done listening for messages.');
      },
    });
  }

  /**
   * Request notification permission from the user
   */
  private async _requestNotificationPermission(): Promise<void> {
    try {
      const permission = await Notification.requestPermission();
      if (permission === 'granted') {
        console.log('Notification permission granted.');
      } else {
        console.warn('Notification permission denied:', permission);
      }
    } catch (error) {
      this._logError('Notification permission request error', error);
    }
  }

  /**
   * Save the device token (for example, send it to the server)
   */
  private _saveDeviceToken(token: string): void {
    console.log('Saving device token:', token);
    this.messagingService.saveToken(token).subscribe({
      error: (ex) => {
        console.log('Token was not saved or updated', ex);
      },
    });
  }

  /**
   * Log errors in a standardized way
   */
  private _logError(message: string, error: any): void {
    console.error(message, error);
    // You can also send these errors to a logging service if needed
  }
}
