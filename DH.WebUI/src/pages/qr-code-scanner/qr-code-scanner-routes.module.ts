import { RouterModule, Routes } from '@angular/router';
import { QrCodeScannerComponent } from './page/qr-code-scanner.component';
import { NgModule } from '@angular/core';
import { AuthGuard } from '../../shared/guards/auth.guard';

const routes: Routes = [
  {
    path: '',
    component: QrCodeScannerComponent,
    canActivate: [AuthGuard],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class QrCodeScannerRoutingModule {}
