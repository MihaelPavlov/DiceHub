import { Component, Inject } from "@angular/core";
import { IQrCode } from "../../../../entities/qr-code-scanner/models/qr-code.model";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";

@Component({
    selector: 'game-reservation-qr-code-dialog',
    templateUrl: 'game-reservation-qr-code-dialog.component.html',
    styleUrls: ['game-reservation-qr-code-dialog.component.scss'],
  })
  export class GameReservationQrCodeDialog {
    constructor(
      @Inject(MAT_DIALOG_DATA) public data: IQrCode,
      private dialogRef: MatDialogRef<GameReservationQrCodeDialog>
    ) {}
  
    public serializeData(): string {
      return JSON.stringify(this.data);
    }
  }
  