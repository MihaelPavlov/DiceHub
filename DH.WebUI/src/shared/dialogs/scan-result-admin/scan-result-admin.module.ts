import { NgModule } from '@angular/core';
import { ScanResultAdminDialog } from './scan-result-admin.dialog';
import { SharedModule } from '../../shared.module';
import { MatDialogActions, MatDialogClose } from '@angular/material/dialog';

@NgModule({
  declarations: [ScanResultAdminDialog],
  exports: [ScanResultAdminDialog],
  providers: [],
  imports: [SharedModule, MatDialogActions, MatDialogClose],
})
export class ScanResultAdminDialogModule {}
