import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { RoomsService } from '../../../../entities/rooms/api/rooms.service';
import { ToastService } from '../../../../shared/services/toast.service';
import { ToastType } from '../../../../shared/models/toast.model';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';

@Component({
  selector: 'app-room-member-confirm-delete-dialog',
  templateUrl: 'room-member-confirm-delete.component.html',
  styleUrl: 'room-member-confirm-delete.component.scss',
})
export class RoomMemberConfirmDeleteDialog {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<RoomMemberConfirmDeleteDialog>,
    private readonly roomService: RoomsService,
    private readonly toastService: ToastService
  ) {}

  public onRemove() {
    this.roomService
      .removeMember(this.data.roomId, this.data.userId)
      .subscribe({
        next: () => {
          this.toastService.success({
            message: 'Member removed successfully',
            type: ToastType.Success,
          });
          this.dialogRef.close(true);
        },
        error: () => {
          this.toastService.error({
            message: AppToastMessage.SomethingWrong,
            type: ToastType.Error,
          });
          this.dialogRef.close(false);
        },
      });
  }
}
