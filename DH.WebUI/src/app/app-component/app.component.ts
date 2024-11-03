import { Observable } from 'rxjs';
import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../entities/auth/auth.service';
import { environment } from '../environment';
import { onMessage } from 'firebase/messaging';
import { Messaging } from '@angular/fire/messaging';
import { MessagingService } from '../../entities/messaging/api/messaging.service';
import { IUserInfo } from '../../entities/auth/models/user-info.model';

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
  constructor(
    private readonly authService: AuthService,
    private readonly _messaging: Messaging,
    private readonly messagingService: MessagingService
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
   * Listen for foreground messages from Firebase Messaging
   */
  private _listenForMessages(): void {
    onMessage(this._messaging, {
      next: (payload) => {
        console.log('Foreground message received:', payload);
        // Handle the message payload here (e.g., display a notification)
      },
      error: (error) => {
        this._logError('Error while listening to messages', error);
      },
      complete: () => {
        console.log('Done listening for messages.');
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
