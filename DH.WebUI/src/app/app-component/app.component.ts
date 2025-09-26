import { BehaviorSubject, filter, map, Observable } from 'rxjs';
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

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent implements OnInit {
  @ViewChild('challengeOverlay') challengeOverlay!: ChallengeOverlayComponent;
  title = 'DH.WebUI';
  public readonly userInfo: Observable<IUserInfo | null> =
    this.authService.userInfo$;
  public areAnyActiveNotificationSubject: BehaviorSubject<boolean> =
    new BehaviorSubject<boolean>(false);
  hideMenu = false;
  constructor(
    private readonly authService: AuthService,
    private readonly _messaging: Messaging,
    private readonly messagingService: MessagingService,
    private readonly notificationService: NotificationsService,
    private readonly cd: ChangeDetectorRef,
    private readonly router: Router,
    private readonly activatedRoute: ActivatedRoute,
    private readonly frontEndLogService: FrontEndLogService,
    private readonly toastService: ToastService
    private readonly challengeHubService: ChallengeHubService
  ) {
    this._initializeUser();
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
      await this.authService.userinfo$();
    }

    await this.challengeHubService.startConnection(
      this.authService.getUser!.id
    );

    this.challengeHubService.onChallengeUpdate((update) => {
      console.log('Challenge progress update:', update.challengeGameName);
      this.challengeOverlay.updateProgress(update.challengeGameName);
    });

    this.challengeHubService.onChallengeCompleted((completed) => {
      console.log('Challenge completed:', completed);
      this.challengeOverlay.completeChallenge(
        completed.challengeGameName,
        completed.rewardPoints
      );
    });

    this.challengeHubService.onRewardGranted((reward) => {
      console.log('Reward granted:', reward);
      this.challengeOverlay.rewardGranted(reward.name_bg, reward.name_en);
    });

    this._initializeFCM();
  }

  /**
   * Initialize Firebase Cloud Messaging related tasks
   */
  private _initializeFCM(): void {
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

      this.messagingService.requestNotificationPermission();
      this.messagingService.getDeviceToken();
      this._listenForMessages();
    }
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
