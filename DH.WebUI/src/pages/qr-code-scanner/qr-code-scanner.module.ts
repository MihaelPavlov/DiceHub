import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { QrCodeScannerComponent } from './page/qr-code-scanner.component';
import { QrCodeScannerRoutingModule } from './qr-code-scanner-routes.module';
import { HeaderModule } from "../../widgets/header/header.module";

@NgModule({
  declarations: [QrCodeScannerComponent],
  exports: [QrCodeScannerComponent],
  providers: [],
  imports: [SharedModule, QrCodeScannerRoutingModule, HeaderModule],
})
export class QrCodeScannerModule {}
