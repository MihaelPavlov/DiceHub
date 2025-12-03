import { Component, Inject, Injectable, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { IQrCode } from '../../../../entities/qr-code-scanner/models/qr-code.model';
import { QrEncryptService } from '../../../../shared/services/qr-code-encrypt.service';

@Component({
    selector: 'user-reward-qr-code-dialog',
    templateUrl: 'user-reward-qr-code.component.html',
    styleUrls: ['user-reward-qr-code.component.scss'],
    standalone: false
})
export class UserRewardQrCodeDialog implements OnInit {
  public encryptedQrData: string | null = null;
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: IQrCode,
    private dialogRef: MatDialogRef<UserRewardQrCodeDialog>,
    private readonly qrEncryptService: QrEncryptService
  ) {}

  public ngOnInit(): void {
    this.encryptedQrData = this.qrEncryptService.encryptObjectSync(this.data);
  }
}
