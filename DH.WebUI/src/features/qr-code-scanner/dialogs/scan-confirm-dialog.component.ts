import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { QrCodeType } from '../../../entities/qr-code-scanner/enums/qr-code-type.enum';

@Component({
    selector: 'app-scan-confirm-dialog',
    templateUrl: 'scan-confirm-dialog.component.html',
    styleUrl: 'scan-confirm-dialog.component.scss',
    standalone: false
})
export class ScanConfirmDialogComponent {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private readonly ts: TranslateService,
    private dialogRef: MatDialogRef<ScanConfirmDialogComponent>
  ) {}

  public get getTranslateType(): string {
    return this.ts.instant(
      `qr_scanner.enum_values.${QrCodeType[this.data.type]}`
    );
  }

  public onConfirm(): void {
    this.dialogRef.close(true);
  }

  public onCancel(): void {
    this.dialogRef.close(false);
  }
}
