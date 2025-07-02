import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared.module';
import {
  MatDialogActions,
  MatDialogClose,
  MatDialogContent,
} from '@angular/material/dialog';
import { UnsavedChangesConfirmationDialog } from './unsaved-changes-confirmation.dialog';
import { NgSelectModule } from '@ng-select/ng-select';

@NgModule({
  declarations: [UnsavedChangesConfirmationDialog],
  exports: [UnsavedChangesConfirmationDialog],
  providers: [],
  imports: [
    SharedModule,
    MatDialogActions,
    MatDialogContent,
    MatDialogClose,
    NgSelectModule,
  ],
})
export class UnsavedChangesConfirmationDialogModule {}
