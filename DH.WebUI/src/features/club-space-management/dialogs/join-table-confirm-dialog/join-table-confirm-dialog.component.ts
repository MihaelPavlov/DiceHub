import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormControl, Validators } from '@angular/forms';
import { SpaceManagementService } from '../../../../entities/space-management/api/space-management.service';

interface JoinTableConfirmDialogData {
  roomId: number;
  withPassword: boolean;
  error: string;
}

@Component({
  selector: 'app-join-table-confirm-dialog',
  templateUrl: 'join-table-confirm-dialog.component.html',
  styleUrl: 'join-table-confirm-dialog.component.scss',
})
export class JoinTableConfirmDialog {
  public password: FormControl = new FormControl('', Validators.required);
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: JoinTableConfirmDialogData,
    private dialogRef: MatDialogRef<JoinTableConfirmDialog>,
    private readonly spaceManagementService: SpaceManagementService
  ) {
    
  }

  public join(): void {
    this.spaceManagementService
      .join(this.data.roomId, this.password.value)
      .subscribe({
        next: () => {
          this.dialogRef.close({ hasError: false });
        },
        error: (error) => {
          if (error.error.errors.Password)
            this.dialogRef.close({
              hasError: true,
              errorMessage: error.error.errors.Password[0],
            });
          if (error.error.errors.MaxPeople) {
            this.dialogRef.close({
              hasError: true,
              errorMessage: error.error.errors.MaxPeople[0],
            });
          }
        },
      });
  }
}
