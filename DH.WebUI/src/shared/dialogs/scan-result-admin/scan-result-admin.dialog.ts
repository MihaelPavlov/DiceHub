import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { IQrCodeValidationResult } from '../../../entities/qr-code-scanner/models/qr-code-validation-result.model';
import { RewardsService } from '../../../entities/rewards/api/rewards.service';
import { ToastService } from '../../services/toast.service';
import { ToastType } from '../../models/toast.model';
import { QrCodeType } from '../../../entities/qr-code-scanner/enums/qr-code-type.enum';

@Component({
  selector: 'scan-result-admin-dialog',
  templateUrl: 'scan-result-admin.dialog.html',
  styleUrls: ['scan-result-admin.dialog.scss'],
})
export class ScanResultAdminDialog {
  public QrCodeType = QrCodeType;
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: IQrCodeValidationResult,
    private dialogRef: MatDialogRef<ScanResultAdminDialog>,
    private readonly rewardsService: RewardsService,
    private readonly toastService: ToastService
  ) {
    console.log('user reward data -> ', data);
  }

  public get getTypeOfQrCode(): string {
    switch (this.data.type) {
      case QrCodeType.Reward:
        return 'Reward';
      case QrCodeType.GameReservation:
        return 'Game Reservation';
      case QrCodeType.Game:
        return 'Game';
      case QrCodeType.Event:
        return 'Event';
      default:
        return 'Qr-Code';
    }
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
