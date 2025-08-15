import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ToastType } from '../../../../shared/models/toast.model';
import { ToastService } from '../../../../shared/services/toast.service';
import { GamesService } from '../../../../entities/games/api/games.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-game-confirm-delete-dialog',
  templateUrl: 'game-confirm-delete.component.html',
  styleUrl: 'game-confirm-delete.component.scss',
})
export class GameConfirmDeleteDialog {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<GameConfirmDeleteDialog>,
    private readonly gameService: GamesService,
    private readonly toastService: ToastService,
    private readonly translateService: TranslateService
  ) {}

  public delete(): void {
    this.gameService.delete(this.data.id).subscribe((_) => {
      this.toastService.success({
        message: this.translateService.instant('deleted'),
        type: ToastType.Success,
      });
      this.dialogRef.close(true);
    });
  }
}
