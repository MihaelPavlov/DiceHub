import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared.module';
import { MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import { ImagePreviewDialog } from './image-preview.dialog';

@NgModule({
  declarations: [ImagePreviewDialog],
  exports: [ImagePreviewDialog],
  providers: [],
  imports: [SharedModule, MatDialogActions, MatDialogClose],
})
export class ImagePreviewDialogModule {}
