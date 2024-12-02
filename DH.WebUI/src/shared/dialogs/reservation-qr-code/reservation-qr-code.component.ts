import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { IQrCode } from '../../../entities/qr-code-scanner/models/qr-code.model';

@Component({
  selector: 'reservation-qr-code-dialog',
  templateUrl: 'reservation-qr-code.component.html',
  styleUrls: ['reservation-qr-code.component.scss'],
})
export class ReservationQrCodeDialog {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: IQrCode,
    private dialogRef: MatDialogRef<ReservationQrCodeDialog>
  ) {}

  public serializeData(): string {
    return JSON.stringify(this.data);
  }
}
