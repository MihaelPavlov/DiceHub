import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { IQrCode } from '../../../../entities/qr-code-scanner/models/qr-code.model';
import { ScannerService } from '../../../../entities/qr-code-scanner/api/scanner.service';

@Component({
  selector: 'user-reward-qr-code-dialog',
  templateUrl: 'user-reward-qr-code.component.html',
  styleUrls: ['user-reward-qr-code.component.scss'],
  standalone: false,
})
export class UserRewardQrCodeDialog implements OnInit {
  public qrData: string | null = null;
  public qrError = false;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: IQrCode,
    private dialogRef: MatDialogRef<UserRewardQrCodeDialog>,
    private readonly scannerService: ScannerService
  ) {}

  public ngOnInit(): void {
    this.scannerService.issueToken(this.data.Type, this.data.Id).subscribe({
      next: (res) => {
        if (res?.token) this.qrData = res.token;
        else this.qrError = true;
      },
      error: () => (this.qrError = true),
    });
  }
}
