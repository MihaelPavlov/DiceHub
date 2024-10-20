import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { IQrCodeValidationResult } from '../../../entities/qr-code-scanner/models/qr-code-validation-result.model';
import { RewardsService } from '../../../entities/rewards/api/rewards.service';
import { ToastService } from '../../services/toast.service';
import { ToastType } from '../../models/toast.model';

@Component({
  selector: 'scan-result-admin-dialog',
  templateUrl: 'scan-result-admin.dialog.html',
  styleUrls: ['scan-result-admin.dialog.scss'],
})
export class ScanResultAdminDialog {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: IQrCodeValidationResult,
    private dialogRef: MatDialogRef<ScanResultAdminDialog>,
    private readonly rewardsService: RewardsService,
    private readonly toastService: ToastService
  ) {
    console.log('user reward data -> ', data);
  }

  public confirm(): void {
    this.rewardsService.userRewardConfirmation(this.data.objectId).subscribe({
      next: () => {
        this.toastService.success({
          message: 'Successfully claimed',
          type: ToastType.Success,
        });

        this.dialogRef.close(true);
      },
      error: () => {
        this.toastService.success({
          message: 'Something went wrong',
          type: ToastType.Error,
        });

        this.dialogRef.close(true);
      },
    });
  }
}
