import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { IQrCodeValidationResult } from '../../../entities/qr-code-scanner/models/qr-code-validation-result.model';
import { RewardsService } from '../../../entities/rewards/api/rewards.service';
import { ToastService } from '../../services/toast.service';
import { ToastType } from '../../models/toast.model';
import { QrCodeType } from '../../../entities/qr-code-scanner/enums/qr-code-type.enum';
import { TranslateService } from '@ngx-translate/core';
import { AppToastMessage } from '../../components/toast/constants/app-toast-messages.constant';

@Component({
    selector: 'scan-result-admin-dialog',
    templateUrl: 'scan-result-admin.dialog.html',
    styleUrls: ['scan-result-admin.dialog.scss'],
    standalone: false
})
export class ScanResultAdminDialog {
  public QrCodeType = QrCodeType;
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: IQrCodeValidationResult,
    private dialogRef: MatDialogRef<ScanResultAdminDialog>,
    private readonly rewardsService: RewardsService,
    private readonly toastService: ToastService,
    private readonly translateService: TranslateService
  ) {}

  public get getTypeOfQrCode(): string {
    switch (this.data.type) {
      case QrCodeType.Reward:
        return this.translateService.instant('qr_scanner.qr_code_type.reward');
      case QrCodeType.GameReservation:
        return this.translateService.instant(
          'qr_scanner.qr_code_type.game_reservation'
        );
      case QrCodeType.Game:
        return this.translateService.instant('qr_scanner.qr_code_type.game');
      case QrCodeType.Event:
        return this.translateService.instant('qr_scanner.qr_code_type.event');
      case QrCodeType.PurchaseChallenge:
        return this.translateService.instant('qr_scanner.qr_code_type.purchase_challenge');
      default:
        return this.translateService.instant('qr_scanner.qr_code_type.qr_code');
    }
  }

  public confirm(): void {
    this.rewardsService.userRewardConfirmation(this.data.objectId).subscribe({
      next: () => {
        this.toastService.success({
          message: this.translateService.instant('qr_scanner.reward_claimed'),
          type: ToastType.Success,
        });

        this.dialogRef.close(true);
      },
      error: () => {
        this.toastService.success({
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
