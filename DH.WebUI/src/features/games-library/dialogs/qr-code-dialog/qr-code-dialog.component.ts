import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { IQrCode } from '../../../../entities/qr-code-scanner/models/qr-code.model';
import { QrCodeType } from '../../../../entities/qr-code-scanner/enums/qr-code-type.enum';
import { QrEncryptService } from '../../../../shared/services/qr-code-encrypt.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'app-game-qr-code-dialog',
    templateUrl: 'qr-code-dialog.component.html',
    styleUrl: 'qr-code-dialog.component.scss',
    standalone: false
})
export class QrCodeDialog implements OnInit{
  public encryptedQrData: string | null = null;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: IQrCode,
    private dialogRef: MatDialogRef<QrCodeDialog>,
    private readonly qrEncryptService: QrEncryptService,
    private readonly translateService: TranslateService
  ) {}

  public ngOnInit(): void {
    this.encryptedQrData = this.qrEncryptService.encryptObjectSync(this.data);
  }

  public get typeLabel(): string {
    switch (this.data?.Type) {
      case QrCodeType.Game:
        return this.translateService.instant('qr_scanner.qr_code_type.game');
      case QrCodeType.Event:
        return this.translateService.instant('qr_scanner.qr_code_type.event');
      case QrCodeType.GameReservation:
      case QrCodeType.TableReservation:
        return this.translateService.instant(
          'qr_scanner.qr_code_type.game_reservation'
        );
      case QrCodeType.PurchaseChallenge:
        return this.translateService.instant(
          'qr_scanner.qr_code_type.purchase_challenge'
        );
      default:
        return this.translateService.instant('qr_scanner.qr_code_type.qr_code');
    }
  }
}
