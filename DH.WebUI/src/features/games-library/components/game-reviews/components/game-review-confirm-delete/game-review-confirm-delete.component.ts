import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { GameReviewsService } from '../../../../../../entities/games/api/game-reviews.service';
import { ToastService } from '../../../../../../shared/services/toast.service';
import { ToastType } from '../../../../../../shared/models/toast.model';

@Component({
  selector: 'app-game-review-confirm-delete-dialog',
  templateUrl: 'game-review-confirm-delete.component.html',
  styleUrl: 'game-review-confirm-delete.component.scss',
})
export class GameReviewConfirmDeleteDialog {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<GameReviewConfirmDeleteDialog>,
    private readonly gameReviewService: GameReviewsService,
    private readonly toastService: ToastService
  ) {}

  public deleteComment() {
    this.gameReviewService.delete(this.data.id).subscribe((_) => {
      this.toastService.success({
        message: 'Deleted',
        type: ToastType.Success,
      });
      this.dialogRef.close(true);
    });
  }
}
