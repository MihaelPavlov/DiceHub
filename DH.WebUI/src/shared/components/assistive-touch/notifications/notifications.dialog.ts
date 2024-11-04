import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ToastService } from '../../../services/toast.service';
import { trigger, transition, style, animate } from '@angular/animations';

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
export class NotificationsDialog {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<NotificationsDialog>,
    private readonly toastService: ToastService
  ) {}

  //TODO: Check if you are be able to delete, because of userChallenges
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
