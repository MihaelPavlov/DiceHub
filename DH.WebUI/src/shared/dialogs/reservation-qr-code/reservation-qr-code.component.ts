import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { IQrCode } from '../../../entities/qr-code-scanner/models/qr-code.model';
import { ScannerService } from '../../../entities/qr-code-scanner/api/scanner.service';

@Component({
  selector: 'reservation-qr-code-dialog',
  templateUrl: 'reservation-qr-code.component.html',
  styleUrls: ['reservation-qr-code.component.scss'],
  standalone: false,
})
export class ReservationQrCodeDialog implements OnInit {
  public qrData: string | null = null;
  public qrError = false;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: IQrCode,
    private dialogRef: MatDialogRef<ReservationQrCodeDialog>,
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
