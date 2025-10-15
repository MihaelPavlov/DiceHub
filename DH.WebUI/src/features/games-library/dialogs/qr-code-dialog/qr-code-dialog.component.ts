import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { IQrCode } from '../../../../entities/qr-code-scanner/models/qr-code.model';
import { QrEncryptService } from '../../../../shared/services/qr-code-encrypt.service';

@Component({
  selector: 'app-game-qr-code-dialog',
  templateUrl: 'qr-code-dialog.component.html',
  styleUrl: 'qr-code-dialog.component.scss',
})
export class QrCodeDialog implements OnInit{
  public encryptedQrData: string | null = null;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: IQrCode,
    private dialogRef: MatDialogRef<QrCodeDialog>,
    private readonly qrEncryptService: QrEncryptService
  ) {}

  public ngOnInit(): void {
    this.encryptedQrData = this.qrEncryptService.encryptObjectSync(this.data);
  }
}
