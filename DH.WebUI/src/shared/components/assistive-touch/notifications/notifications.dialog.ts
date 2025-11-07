import {
  Component,
  ElementRef,
  EventEmitter,
  Inject,
  OnInit,
  ViewChild,
} from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { trigger, transition, style, animate } from '@angular/animations';
import { NotificationsService } from '../../../../entities/common/api/notifications.service';
import { IUserNotification } from '../../../../entities/common/models/user-notification-model';
import { DateHelper } from '../../../helpers/date-helper';
import { MessagingService } from '../../../../entities/messaging/api/messaging.service';
import { LanguageService } from '../../../services/language.service';
import { SupportLanguages } from '../../../../entities/common/models/support-languages.enum';
import { Router } from '@angular/router';

@Component({
  selector: 'app-notifications-dialog',
  templateUrl: 'notifications.dialog.html',
  styleUrl: 'notifications.dialog.scss',
  animations: [
    trigger('dialogAnimation', [
      transition(':enter', [
        style({ transform: 'scaleY(0.005) scaleX(0)' }),
        animate(
          '1s cubic-bezier(0.165, 0.84, 0.44, 1)',
          style({ transform: 'scaleY(1) scaleX(1)' })
        ),
      ]),
      transition(':leave', [
        animate(
          '1s cubic-bezier(0.165, 0.84, 0.44, 1)',
          style({ transform: 'scaleY(0.005) scaleX(0)' })
        ),
      ]),
    ]),
    trigger('modalAnimation', [
      transition(':enter', [
        style({ transform: 'scale(0)' }),
        animate(
          '0.5s 0.8s cubic-bezier(0.165, 0.84, 0.44, 1)',
          style({ transform: 'scale(1)' })
        ),
      ]),
      transition(':leave', [
        animate(
          '0.5s cubic-bezier(0.165, 0.84, 0.44, 1)',
          style({ transform: 'scale(0)' })
        ),
      ]),
    ]),
  ],
})
export class NotificationsDialog implements OnInit {
  public notificationsUpdated = new EventEmitter<IUserNotification[]>();
  public userNotifications: IUserNotification[] = [];
  public page = 1;
  public pageSize = 20;
  public loading = false;
  public allLoaded = false;
  public readonly DATE_TIME_FORMAT: string = DateHelper.DATE_TIME_FORMAT;
  public notificationPermission: NotificationPermission = 'default';
  public pushUnsupported: boolean = false;
  public showWarning = true;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<NotificationsDialog>,
    private readonly notificationService: NotificationsService,
    private readonly messagingService: MessagingService,
    private readonly languageService: LanguageService,
    private readonly router: Router
  ) {}

  public ngOnInit(): void {
    this.loadNotifications();
    this.notificationPermission = Notification.permission;
    this.pushUnsupported = this.messagingService.isPushUnsupportedIOS();
    const dismissed = localStorage.getItem('pushUnsupportedWarningDismissed');
    this.showWarning = !dismissed;
  }

  @ViewChild('infiniteScrollTrigger', { static: false })
  infiniteScrollTrigger!: ElementRef;

  public get currentLanguage(): SupportLanguages {
    return this.languageService.getCurrentLanguage();
  }

  public ngAfterViewInit() {
    if (!this.infiniteScrollTrigger) return;

    const observer = new IntersectionObserver((entries) => {
      if (entries[0].isIntersecting) {
        this.loadNotifications();
      }
    });
    observer.observe(this.infiniteScrollTrigger.nativeElement);
  }

  public loadNotifications(refresh: boolean = false): void {
    // Reset if refreshing
    if (refresh) {
      this.page = 1;
      this.allLoaded = false; // <-- reset this so it can load again
      this.userNotifications.length = 0;
    }
    // Prevent further calls if we already loaded everything
    if (this.allLoaded) {
      return;
    }
    this.loading = true;

    this.notificationService
      .getUserNotificationList(this.page, this.pageSize)
      .subscribe({
        next: (result) => {
          if (result.length < this.pageSize) {
            this.allLoaded = true; // no more data
          }
          this.userNotifications.push(...result);
          this.page++;
          this.loading = false;

          if (refresh) this.notificationsUpdated.emit();
        },
        error: () => {
          this.loading = false;
        },
      });
  }

  public dismissWarning() {
    localStorage.setItem('pushUnsupportedWarningDismissed', 'true');
    this.showWarning = false;
  }

  public navigateToEnableBrowserNotificationInstruction(): void {
    this.router.navigateByUrl('instructions/notifications');
    this.dialogRef.close();
  }

  public get isAllMarkedAsViewed(): boolean {
    return this.userNotifications.every((x) => x.hasBeenViewed);
  }

  public trackByNotification(index: number, item: IUserNotification) {
    return item.id; // or unique key
  }

  public markedAsViewed(id: number, hasBeenViewed: boolean) {
    if (!hasBeenViewed)
      this.notificationService.markNotificationAsViewed(id).subscribe({
        next: () => {
          const notification = this.userNotifications.find((n) => n.id === id);
          if (notification) notification.hasBeenViewed = true; // update locally
        },
      });
  }

  public clearAll(): void {
    this.notificationService.clearUserAllNotification().subscribe({
      next: () => {
        this.loadNotifications(true);
      },
    });
  }

  public markAll(): void {
    this.notificationService.markedAsViewAllUserNotification().subscribe({
      next: () => {
        this.loadNotifications(true);
      },
    });
  }

  public closeDialog(): void {
    document
      .getElementsByClassName('animate__animated')[0]
      .classList.remove('animate__slideInLeft');
    document
      .getElementsByClassName('animate__animated')[0]
      .classList.add('animate__slideOutRight');
    setTimeout(() => {
      this.dialogRef.close();
    }, 1000);
  }
}
