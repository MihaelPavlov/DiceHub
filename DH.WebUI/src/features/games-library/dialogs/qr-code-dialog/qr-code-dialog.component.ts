import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ToastService } from '../../../../shared/services/toast.service';
import { GamesService } from '../../../../entities/games/api/games.service';
import { IGameQrCode } from '../../../../entities/games/models/game-qr-code.model';
import { combineLatest } from 'rxjs';
import { IGameByIdResult } from '../../../../entities/games/models/game-by-id.model';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../../shared/models/toast.model';

@Component({
  selector: 'app-game-qr-code-dialog',
  templateUrl: 'qr-code-dialog.component.html',
  styleUrl: 'qr-code-dialog.component.scss',
})
export class GameQrCodeDialog implements OnInit {
  public qrCode!: IGameQrCode;
  public game!: IGameByIdResult;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<GameQrCodeDialog>,
    private readonly gameService: GamesService,
    private readonly toastService: ToastService
  ) {}

  public ngOnInit(): void {
    combineLatest([
      this.gameService.getQrCode(this.data.id),
      this.gameService.getById(this.data.id),
    ]).subscribe({
      next: ([qrCode, game]) => {
        if (qrCode && game) {
          this.game = game;
          this.qrCode = qrCode;
        } else {
          this.dialogRef.close();
        }
      },
      error: (error) => {
        this.toastService.error({
          message: AppToastMessage.SomethingWrong,
          type: ToastType.Error,
        });
        this.dialogRef.close();
      },
    });
  }
}
