import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { IQrCode } from '../../../entities/qr-code-scanner/models/qr-code.model';
import { QrEncryptService } from '../../services/qr-code-encrypt.service';

@Component({
    selector: 'reservation-qr-code-dialog',
    templateUrl: 'reservation-qr-code.component.html',
    styleUrls: ['reservation-qr-code.component.scss'],
    standalone: false
})
export class ReservationQrCodeDialog implements OnInit {
  public encryptedQrData: string | null = null;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: IQrCode,
    private dialogRef: MatDialogRef<ReservationQrCodeDialog>,
    private readonly qrEncryptService: QrEncryptService
  ) {}
  
  public ngOnInit(): void {
    this.encryptedQrData = this.qrEncryptService.encryptObjectSync(this.data);
  }
}
