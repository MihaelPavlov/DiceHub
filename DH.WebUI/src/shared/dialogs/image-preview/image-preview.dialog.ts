import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

export interface ImagePreviewData {
  imageUrl: string;
  title?: string;
  removeHeight?: boolean;
}

@Component({
    selector: 'image-preview',
    templateUrl: 'image-preview.dialog.html',
    styleUrls: ['image-preview.dialog.scss'],
    standalone: false
})
export class ImagePreviewDialog {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: ImagePreviewData,
    private dialogRef: MatDialogRef<ImagePreviewDialog>
  ) {}
}
