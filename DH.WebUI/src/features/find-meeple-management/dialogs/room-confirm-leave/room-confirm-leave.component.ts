import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { RoomsService } from '../../../../entities/rooms/api/rooms.service';
import { ToastService } from '../../../../shared/services/toast.service';
import { ToastType } from '../../../../shared/models/toast.model';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'app-room-confirm-leave-dialog',
    templateUrl: 'room-confirm-leave.component.html',
    styleUrl: 'room-confirm-leave.component.scss',
    standalone: false
})
export class RoomConfirmLeaveDialog {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<RoomConfirmLeaveDialog>,
    private readonly roomService: RoomsService,
    private readonly toastService: ToastService,
    private readonly translateService: TranslateService
  ) {}

  public onLeave() {
    this.roomService.leave(this.data.id).subscribe({
      next: () => {
        this.toastService.success({
          message: this.translateService.instant(
            'meeple.room.left_room_success'
          ),
          type: ToastType.Success,
        });
        this.dialogRef.close(true);
      },
      error: () => {
        this.toastService.error({
          message: this.translateService.instant(
            AppToastMessage.SomethingWrong
          ),
          type: ToastType.Error,
        });
        this.dialogRef.close(true);
      },
    });
  }
}
