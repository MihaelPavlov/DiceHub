import { BehaviorSubject, catchError, filter, map, Observable, of } from 'rxjs';
import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { AuthService } from '../../entities/auth/auth.service';
import { onMessage } from 'firebase/messaging';
import { Messaging } from '@angular/fire/messaging';
import { MessagingService } from '../../entities/messaging/api/messaging.service';
import { IUserInfo } from '../../entities/auth/models/user-info.model';
import { NotificationsService } from '../../entities/common/api/notifications.service';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { FrontEndLogService } from '../../shared/services/frontend-log.service';
import { ChallengeHubService } from '../../entities/challenges/api/challenge-hub.service';
import { ChallengeOverlayComponent } from '../../shared/components/challenge-overlay/challenge-overlay.component';
import { ChallengeOverlayService } from '../../shared/services/challenges-overlay.service';
import { Capacitor } from '@capacitor/core';
import { App } from '@capacitor/app';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
  standalone: false,
})
export class AppComponent implements OnInit {
  @ViewChild('challengeOverlay') challengeOverlay!: ChallengeOverlayComponent;
  title = 'DH.WebUI';
  public readonly userInfo: Observable<IUserInfo | null> =
    this.authService.userInfo$;
  public areAnyActiveNotificationSubject: BehaviorSubject<boolean> =
    new BehaviorSubject<boolean>(false);
  hideMenu = false;
  public isSystemAdmin = false;

  constructor(
    private readonly authService: AuthService,
    private readonly _messaging: Messaging,
    private readonly messagingService: MessagingService,
    private readonly notificationService: NotificationsService,
    private readonly cd: ChangeDetectorRef,
    private readonly router: Router,
    private readonly activatedRoute: ActivatedRoute,
    private readonly frontEndLogService: FrontEndLogService,
    private readonly challengeHubService: ChallengeHubService,
    private readonly challengeOverlayService: ChallengeOverlayService
  ) {
    window.addEventListener(
      'touchstart',
      (e) => {
        if (e.touches.length === 1) {
          const touch = e.touches[0];
          // Save initial position if you want to detect swipe direction
          window['touchStartX'] = touch.clientX;
        }
      },
      { passive: false }
    );

    window.addEventListener(
      'touchmove',
      (e) => {
        if (e.touches.length === 1) {
          const touch = e.touches[0];
          const diffX = touch.clientX - window['touchStartX'];

          // If horizontal swipe more than threshold, prevent default to block back/forward navigation
          if (Math.abs(diffX) > 30) {
            e.preventDefault();
          }
        }
      },
      { passive: false }
    );
    this.router.events
      .pipe(
        filter((event) => event instanceof NavigationEnd),
        map(() => {
          let route = this.activatedRoute;
          while (route.firstChild) {
            route = route.firstChild;
          }
          return route;
        }),
        map((route) => route.snapshot.data['hideMenu'])
      )
      .subscribe((hideMenu: boolean) => {
        this.hideMenu = hideMenu;
      });

    this._initializeAndroidBackButton();
    this._persistRouteForRestoration();
    this._initializeAppUrlOpenListener();
  }

  /**
   * Android App Links (see AndroidManifest.xml's autoVerify intent-filter for
   * dicehubs.com) launch this app for a matching https://dicehubs.com/... link
   * clicked anywhere (email, SMS, other apps) - but Capacitor just opens the
   * Activity, it doesn't know this app is a client-routed SPA. Without this,
   * a tenant-setup/reset-password/etc. email link would open the app to
   * whatever its normal default screen is, not the actual linked page. The
   * WebView runs on its own local origin, not dicehubs.com, so only the
   * path/query/hash from the incoming URL is meaningful here.
   */
  private _initializeAppUrlOpenListener(): void {
    if (!Capacitor.isNativePlatform()) {
      return;
    }

    App.addListener('appUrlOpen', (data: { url: string }) => {
      try {
        const url = new URL(data.url);
        this.router.navigateByUrl(url.pathname + url.search + url.hash);
      } catch {
        // Malformed/unexpected URL - nothing sensible to navigate to.
      }
    });
  }

  /**
   * Android can kill the app's background process at any time to reclaim memory
   * (trivially triggered by this app's own file/photo pickers, e.g. the venue-application
   * logo upload). On relaunch, Capacitor reloads index.html fresh with no memory of which
   * in-app route was active, so the router's first navigation always lands on ''. Persist
   * every real navigation so a cold boot can send the user back where they were instead of
   * the landing page. Web (non-native) behavior is untouched - a browser refresh at '/' is
   * expected to show the landing page there.
   */
  private static readonly RouteRestorationExcludedSegments = [
    '/reset-password',
    '/confirm-email',
    '/create-employee-password',
    '/create-owner-password',
    '/login',
  ];
  private static readonly LastRouteStorageKey = 'lastRoute';

  private _persistRouteForRestoration(): void {
    if (!Capacitor.isNativePlatform()) {
      return;
    }

    this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe((event) => {
        const url = (event as NavigationEnd).urlAfterRedirects;

        if (
          url === '/' ||
          AppComponent.RouteRestorationExcludedSegments.some((segment) =>
            url.includes(segment)
          )
        ) {
          return;
        }

        localStorage.setItem(AppComponent.LastRouteStorageKey, url);
      });
  }

  /**
   * Restores Capacitor's default hardware/gesture back-button behavior on Android:
   * go back through in-app navigation history, or exit the app if there's none left.
   * Without this, the WebView has no back-button handler at all and every press exits the app.
   */
  private _initializeAndroidBackButton(): void {
    if (!Capacitor.isNativePlatform()) {
      return;
    }

    App.addListener('backButton', ({ canGoBack }) => {
      if (canGoBack) {
        window.history.back();
      } else {
        App.exitApp();
      }
    });
  }

  // TODO: Check this tread https://chatgpt.com/c/671602c4-266c-800d-8177-2e9b398333ba
  public async ngOnInit(): Promise<void> {
    await this._initializeUser();
  }

  /**
   * Initialize the user on component load
   */
  private async _initializeUser(): Promise<void> {
    if (!this.authService.getUser) {
      console.log('unit user');

      await this.authService.userinfo$();
    }

    this.isSystemAdmin = this.authService.getUser?.tenantId === 'system';

    if (this.authService.getUser?.tenantId === 'system') {
      this.challengeOverlayService.init(this.challengeOverlay);
      return;
    }

    if (this.authService.getUser) {
      await this.challengeHubService.initChallengeHubConnection(
        this.authService.getUser.id,
        this.challengeOverlay
      );
    } else {
      this.challengeOverlayService.init(this.challengeOverlay);
    }

    this._initializeFCM();
  }

  /**
   * Initialize Firebase Cloud Messaging related tasks
   */
  private _initializeFCM(): void {
    if (this.authService.getUser?.tenantId === 'system') {
      return;
    }

    if (this.authService.getUser) {
      if (this.messagingService.isPushUnsupportedIOS()) {
        this.frontEndLogService
          .sendWarning(
            'Push notifications not supported on this iOS version',
            'none'
          )
          .subscribe();
        return;
      }

      this.frontEndLogService
        .sendInfo('Initializing Firebase Cloud Messaging...', 'none')
        .subscribe();

      this.messagingService.getDeviceToken();
      this._listenForMessages();
    }
  }

  public onUpdateUserNotifications() {
    if (this.authService.getUser?.tenantId === 'system') {
      return;
    }

    this.notificationService
      .areAnyActiveNotifications()
      .pipe(
        catchError((err) => {
          console.warn('Are Any Active Notifications failed silently', err);
          return of(false);
        })
      )
      .subscribe({
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
      next: (res) => {
        console.log('Received foreground message:', res);

        this.notificationService.areAnyActiveNotifications().subscribe({
          next: (result) => {
            console.log('------------Are any active notifications:', result);
            this.areAnyActiveNotificationSubject.next(result);
            this.cd.detectChanges();
          },
        });
      },
      error: (error) => {
        console.log('Error receiving message:', error);
      },
      complete: () => {
        console.log('Done listening for messages.');
      },
    });
  }
}
