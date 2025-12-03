import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared.module';
import { MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import { ReservationQrCodeDialog } from './reservation-qr-code.component';
import { QRCodeComponent } from 'angularx-qrcode';

@NgModule({
  declarations: [ReservationQrCodeDialog],
  exports: [ReservationQrCodeDialog],
  providers: [],
  imports: [SharedModule, MatDialogActions, MatDialogClose, QRCodeComponent ],
})
export class ReservationQrCodeDialogModule {}
