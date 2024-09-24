import { Component, OnDestroy } from '@angular/core';
import { MenuTabsService } from '../../../../../shared/services/menu-tabs.service';
import { NAV_ITEM_LABELS } from '../../../../../shared/models/nav-items-labels.const';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { GamesService } from '../../../../../entities/games/api/games.service';
import { IGameQrCode } from '../../../../../entities/games/models/game-qr-code.model';
import { Observable } from 'rxjs';
import { ToastService } from '../../../../../shared/services/toast.service';
import { ToastType } from '../../../../../shared/models/toast.model';
import { AppToastMessage } from '../../../../../shared/components/toast/constants/app-toast-messages.constant';
import { IGameByIdResult } from '../../../../../entities/games/models/game-by-id.model';

@Component({
  selector: 'app-game-qr-code-list',
  templateUrl: 'game-qr-code-list.component.html',
  styleUrl: 'game-qr-code-list.component.scss',
})
export class GameQrCodeListComponent implements OnDestroy {
  public isMenuVisible: boolean = false;
  public qrCodes$!: Observable<IGameQrCode[]>;
  public game!: IGameByIdResult;
  private gameId!: number;

  constructor(
    private readonly menuTabsService: MenuTabsService,
    private readonly gameService: GamesService,
    private readonly router: Router,
    private readonly activeRoute: ActivatedRoute,
    private readonly toastService: ToastService
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.GAMES);
  }

  public ngOnInit(): void {
    this.activeRoute.params.subscribe((params: Params) => {
      this.gameId = params['id'];
      // this.qrCodes$ = this.gameService.getQrCodes(this.gameId);
      this.gameService.getById(this.gameId).subscribe({
        next: (game) => {
          this.game = game;
          if (!game) {
            this.toastService.error({
              message: AppToastMessage.SomethingWrong,
              type: ToastType.Error,
            });
          }
        },
        error: () => {
          this.toastService.error({
            message: AppToastMessage.SomethingWrong,
            type: ToastType.Error,
          });
        },
      });
    });
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public backNavigateBtn() {
    this.router.navigateByUrl(`games/library`);
  }

  public showMenu(): void {
    this.isMenuVisible = !this.isMenuVisible;
  }
}
