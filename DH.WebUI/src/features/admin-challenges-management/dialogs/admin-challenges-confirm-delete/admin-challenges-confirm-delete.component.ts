import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ToastType } from '../../../../shared/models/toast.model';
import { ToastService } from '../../../../shared/services/toast.service';
import { ChallengesService } from '../../../../entities/challenges/api/challenges.service';

@Component({
  selector: 'app-admin-challenges-confirm-delete-dialog',
  templateUrl: 'admin-challenges-confirm-delete.component.html',
  styleUrl: 'admin-challenges-confirm-delete.component.scss',
})
export class AdminChallengesConfirmDeleteDialog {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<AdminChallengesConfirmDeleteDialog>,
    private readonly challengesService: ChallengesService,
    private readonly toastService: ToastService
  ) {}

  //TODO: Check if you are be able to delete, because of userChallenges
  public delete(): void {
    this.challengesService.delete(this.data.id).subscribe((_) => {
      this.toastService.success({
        message: 'Deleted',
        type: ToastType.Success,
      });
      this.dialogRef.close(true);
    });
  }
}
