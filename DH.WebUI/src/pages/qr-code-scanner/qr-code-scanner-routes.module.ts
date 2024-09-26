import { RouterModule, Routes } from '@angular/router';
import { QrCodeScannerComponent } from './page/qr-code-scanner.component';
import { NgModule } from '@angular/core';

const routes: Routes = [
  {
    path: '',
    component: QrCodeScannerComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class QrCodeScannerRoutingModule {}
