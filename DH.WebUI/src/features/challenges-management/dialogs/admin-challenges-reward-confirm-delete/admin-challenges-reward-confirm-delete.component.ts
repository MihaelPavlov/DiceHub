import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ToastType } from '../../../../shared/models/toast.model';
import { ToastService } from '../../../../shared/services/toast.service';
import { RewardsService } from '../../../../entities/rewards/api/rewards.service';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';

@Component({
  selector: 'app-admin-challenges-reward-confirm-delete-dialog',
  templateUrl: 'admin-challenges-reward-confirm-delete.component.html',
  styleUrl: 'admin-challenges-reward-confirm-delete.component.scss',
})
export class AdminChallengesRewardConfirmDeleteDialog {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<AdminChallengesRewardConfirmDeleteDialog>,
    private readonly rewardService: RewardsService,
    private readonly toastService: ToastService
  ) {}

  public delete(): void {
    this.rewardService.delete(this.data.id).subscribe({
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
        this.dialogRef.close(false);
      },
    });
  }
}
