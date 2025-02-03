import { Component, EventEmitter, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { trigger, transition, style, animate } from '@angular/animations';
import { NotificationsService } from '../../../../entities/common/api/notifications.service';
import { IUserNotification } from '../../../../entities/common/models/user-notification-model';
import { DateHelper } from '../../../helpers/date-helper';

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

  public readonly DATE_TIME_FORMAT: string = DateHelper.DATE_TIME_FORMAT;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<NotificationsDialog>,
    private readonly notificationService: NotificationsService
  ) {}

  public ngOnInit(): void {
    this.notificationService.getUserNotificationList().subscribe({
      next: (result) => {
        this.userNotifications = result;
        console.log(this.userNotifications);
      },
    });
  }

  public markedAsViewed(id: number, hasBeenViewed: boolean) {
    if (!hasBeenViewed)
      this.notificationService.markNotificationAsViewed(id).subscribe({
        next: () => {
          this.notificationService.getUserNotificationList().subscribe({
            next: (result) => {
              this.userNotifications = result;
              this.notificationsUpdated.emit();
            },
          });
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
