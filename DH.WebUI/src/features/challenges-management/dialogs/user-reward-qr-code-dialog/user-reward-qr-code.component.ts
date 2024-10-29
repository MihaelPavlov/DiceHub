import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { IQrCode } from '../../../../entities/qr-code-scanner/models/qr-code.model';

@Component({
  selector: 'user-reward-qr-code-dialog',
  templateUrl: 'user-reward-qr-code.component.html',
  styleUrls: ['user-reward-qr-code.component.scss'],
})
export class UserRewardQrCodeDialog {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: IQrCode,
    private dialogRef: MatDialogRef<UserRewardQrCodeDialog>
  ) {}

  public serializeData(): string {
    return JSON.stringify(this.data);
  }
}
