import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared.module';
import { MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import { ReservationQrCodeDialog } from './reservation-qr-code.component';
import { QRCodeModule } from 'angularx-qrcode';

@NgModule({
  declarations: [ReservationQrCodeDialog],
  exports: [ReservationQrCodeDialog],
  providers: [],
  imports: [SharedModule, MatDialogActions, MatDialogClose, QRCodeModule],
})
export class ReservationQrCodeDialogModule {}
