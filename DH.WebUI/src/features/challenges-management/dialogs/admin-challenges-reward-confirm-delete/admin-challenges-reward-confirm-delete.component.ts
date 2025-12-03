import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ToastType } from '../../../../shared/models/toast.model';
import { ToastService } from '../../../../shared/services/toast.service';
import { RewardsService } from '../../../../entities/rewards/api/rewards.service';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'app-admin-challenges-reward-confirm-delete-dialog',
    templateUrl: 'admin-challenges-reward-confirm-delete.component.html',
    styleUrl: 'admin-challenges-reward-confirm-delete.component.scss',
    standalone: false
})
export class AdminChallengesRewardConfirmDeleteDialog {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<AdminChallengesRewardConfirmDeleteDialog>,
    private readonly rewardService: RewardsService,
    private readonly toastService: ToastService,
    private readonly ts: TranslateService
  ) {}

  public delete(): void {
    this.rewardService.delete(this.data.id).subscribe({
      next: () => {
        this.toastService.success({
          message: this.ts.instant('deleted'),
          type: ToastType.Success,
        });
        this.dialogRef.close(true);
      },
      error: () => {
        this.toastService.error({
          message: this.ts.instant(AppToastMessage.SomethingWrong),
          type: ToastType.Error,
        });
        this.dialogRef.close(false);
      },
    });
  }
}
