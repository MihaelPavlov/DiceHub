import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ToastType } from '../../../../shared/models/toast.model';
import { ToastService } from '../../../../shared/services/toast.service';
import { ChallengesService } from '../../../../entities/challenges/api/challenges.service';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';

@Component({
  selector: 'app-admin-challenges-confirm-delete-dialog',
  templateUrl: 'admin-challenges-confirm-delete.component.html',
  styleUrl: 'admin-challenges-confirm-delete.component.scss',
})
export class AdminChallengesConfirmDeleteDialog {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<AdminChallengesConfirmDeleteDialog>,
    private readonly challengesService: ChallengesService,
    private readonly toastService: ToastService
  ) {}

  public delete(): void {
    this.challengesService.delete(this.data.id).subscribe({
      next: () => {
        this.toastService.success({
          message: 'Deleted',
          type: ToastType.Success,
        });
        this.dialogRef.close(true);
      },
      error: (error: any) => {
        if (error.error.errors.UserChallenges) {
          this.toastService.error({
            message: error.error.errors.UserChallenges[0],
            type: ToastType.Error,
          });
        } else {
          this.toastService.error({
            message: AppToastMessage.SomethingWrong,
            type: ToastType.Error,
          });
        }
        this.dialogRef.close(false);
      },
    });
  }
}
