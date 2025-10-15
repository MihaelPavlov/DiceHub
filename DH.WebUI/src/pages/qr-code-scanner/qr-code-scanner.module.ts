import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { QrCodeScannerComponent } from './page/qr-code-scanner.component';
import { QrCodeScannerRoutingModule } from './qr-code-scanner-routes.module';
import { HeaderModule } from '../../widgets/header/header.module';
import { ScanResultAdminDialogModule } from '../../shared/dialogs/scan-result-admin/scan-result-admin.module';
import { ScanConfirmDialogComponent } from '../../features/qr-code-scanner/dialogs/scan-confirm-dialog.component';

@NgModule({
  declarations: [QrCodeScannerComponent, ScanConfirmDialogComponent],
  exports: [QrCodeScannerComponent],
  providers: [],
  imports: [
    SharedModule,
    QrCodeScannerRoutingModule,
    HeaderModule,
    ScanResultAdminDialogModule,
  ],
})
export class QrCodeScannerModule {}
