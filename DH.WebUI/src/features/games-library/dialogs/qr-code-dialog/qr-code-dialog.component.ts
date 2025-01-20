import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { IQrCode } from '../../../../entities/qr-code-scanner/models/qr-code.model';

@Component({
  selector: 'app-game-qr-code-dialog',
  templateUrl: 'qr-code-dialog.component.html',
  styleUrl: 'qr-code-dialog.component.scss',
})
export class GameQrCodeDialog {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: IQrCode,
    private dialogRef: MatDialogRef<GameQrCodeDialog>
  ) {}

  public serializeData(): string {
    return JSON.stringify(this.data);
  }
}
