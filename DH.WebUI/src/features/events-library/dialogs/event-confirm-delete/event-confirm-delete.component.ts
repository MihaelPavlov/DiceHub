import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ToastType } from '../../../../shared/models/toast.model';
import { ToastService } from '../../../../shared/services/toast.service';
import { EventsService } from '../../../../entities/events/api/events.service';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';

@Component({
  selector: 'app-event-confirm-delete-dialog',
  templateUrl: 'event-confirm-delete.component.html',
  styleUrl: 'event-confirm-delete.component.scss',
})
export class EventConfirmDeleteDialog {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<EventConfirmDeleteDialog>,
    private readonly eventsService: EventsService,
    private readonly toastService: ToastService
  ) {}

  public delete(): void {
    this.eventsService.delete(this.data.id).subscribe({
      next: () => {
        this.toastService.success({
          message: 'Deleted',
          type: ToastType.Success,
        });
        this.dialogRef.close(true);
      },
      error: () => {
        this.toastService.error({
          message: AppToastMessage.SomethingWrong,
          type: ToastType.Error,
        });
      },
    });
  }
}
