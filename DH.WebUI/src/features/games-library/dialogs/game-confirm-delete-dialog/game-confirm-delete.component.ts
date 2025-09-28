import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ToastType } from '../../../../shared/models/toast.model';
import { ToastService } from '../../../../shared/services/toast.service';
import { GamesService } from '../../../../entities/games/api/games.service';
import { TranslateService } from '@ngx-translate/core';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';

@Component({
  selector: 'app-game-confirm-delete-dialog',
  templateUrl: 'game-confirm-delete.component.html',
  styleUrl: 'game-confirm-delete.component.scss',
})
export class GameConfirmDeleteDialog {
  public errorMessage: string | null = null;
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<GameConfirmDeleteDialog>,
    private readonly gameService: GamesService,
    private readonly toastService: ToastService,
    private readonly translateService: TranslateService
  ) {}

  public delete(): void {
    this.gameService.delete(this.data.id).subscribe({
      next: () => {
        this.toastService.success({
          message: this.translateService.instant('deleted'),
          type: ToastType.Success,
        });
        this.dialogRef.close(true);
      },
      error: (error: any) => {
        if (error.error.errors.ActiveGameReservations)
          this.errorMessage = error.error.errors.ActiveGameReservations[0];
        else if (error.error.errors.ActiveEvents)
          this.errorMessage = error.error.errors.ActiveEvents[0];
        else if (error.error.errors.ActiveCustomPeriod)
          this.errorMessage = error.error.errors.ActiveCustomPeriod[0];
        else if (error.error.errors.ActiveChallenges)
          this.errorMessage = error.error.errors.ActiveChallenges[0];

        this.toastService.error({
          message: this.translateService.instant(
            AppToastMessage.SomethingWrong
          ),
          type: ToastType.Error,
        });
      },
    });
  }
}
