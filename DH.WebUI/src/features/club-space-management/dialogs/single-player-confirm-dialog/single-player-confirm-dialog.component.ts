import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ToastService } from '../../../../shared/services/toast.service';
import { GamesService } from '../../../../entities/games/api/games.service';

@Component({
  selector: 'app-single-player-confirm-dialog',
  templateUrl: 'single-player-confirm-dialog.component.html',
  styleUrl: 'single-player-confirm-dialog.component.scss',
})
export class SinglePlayerConfirmDialog {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<SinglePlayerConfirmDialog>,
    private readonly gameService: GamesService,
    private readonly toastService: ToastService
  ) {}

  public onSinglePlayer(): void {
    this.dialogRef.close(true);
  }
}
