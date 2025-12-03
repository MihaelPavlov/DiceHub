import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ToastType } from '../../../../shared/models/toast.model';
import { ToastService } from '../../../../shared/services/toast.service';
import { EventsService } from '../../../../entities/events/api/events.service';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'app-event-confirm-delete-dialog',
    templateUrl: 'event-confirm-delete.component.html',
    styleUrl: 'event-confirm-delete.component.scss',
    standalone: false
})
export class EventConfirmDeleteDialog {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<EventConfirmDeleteDialog>,
    private readonly eventsService: EventsService,
    private readonly toastService: ToastService,
    private readonly translateService: TranslateService
  ) {}

  public delete(): void {
    this.eventsService.delete(this.data.id).subscribe({
      next: () => {
        this.toastService.success({
          message: this.translateService.instant('deleted'),
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
      },
    });
  }
}
