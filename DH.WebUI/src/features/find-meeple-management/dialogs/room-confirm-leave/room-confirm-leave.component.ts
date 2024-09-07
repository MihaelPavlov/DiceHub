import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { RoomsService } from '../../../../entities/rooms/api/rooms.service';
import { ToastService } from '../../../../shared/services/toast.service';
import { ToastType } from '../../../../shared/models/toast.model';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';

@Component({
  selector: 'app-room-confirm-leave-dialog',
  templateUrl: 'room-confirm-leave.component.html',
  styleUrl: 'room-confirm-leave.component.scss',
})
export class RoomConfirmLeaveDialog {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<RoomConfirmLeaveDialog>,
    private readonly roomService: RoomsService,
    private readonly toastService: ToastService
  ) {}

  public onLeave() {
    this.roomService.leave(this.data.id).subscribe({
      next: () => {
        this.toastService.success({
          message: 'Room successfully left',
          type: ToastType.Success,
        });
        this.dialogRef.close(true);
      },
      error: (error) => {
        this.toastService.error({
          message: AppToastMessage.SomethingWrong,
          type: ToastType.Error,
        });
        this.dialogRef.close(true);
      },
    });
  }
}
