import {
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
} from '@angular/core';
import { IGameByIdResult } from '../../../../../entities/games/models/game-by-id.model';
import { GamesService } from '../../../../../entities/games/api/games.service';
import { MenuTabsService } from '../../../../../shared/services/menu-tabs.service';
import { NavItemInterface } from '../../../../../shared/models/nav-item.mode';
import { MatDialog } from '@angular/material/dialog';
import { GameQrCodeDialog } from '../../../dialogs/qr-code-dialog/qr-code-dialog.component';
import { AuthService } from '../../../../../entities/auth/auth.service';
import { UserRole } from '../../../../../entities/auth/enums/roles.enum';
import { FULL_ROUTE } from '../../../../../shared/configs/route.config';
import { QrCodeType } from '../../../../../entities/qr-code-scanner/enums/qr-code-type.enum';
import { ImageEntityType } from '../../../../../shared/pipe/entity-image.pipe';
import { TranslateService } from '@ngx-translate/core';
import {
  ImagePreviewData,
  ImagePreviewDialog,
} from '../../../../../shared/dialogs/image-preview/image-preview.dialog';

@Component({
  selector: 'app-game-layout',
  templateUrl: 'game-layout.component.html',
  styleUrl: 'game-layout.component.scss',
})
export class GameLayoutComponent implements OnInit, OnDestroy {
  @Input() game!: IGameByIdResult;
  @Input() backNavigateBtn: () => void = () => {};
  @Output() refresh = new EventEmitter<void>();

  public isQrCodeVisible: boolean =
    this.authService.getUser?.role !== UserRole.User;
  public menuItems: NavItemInterface[] = [];
  public readonly ImageEntityType = ImageEntityType;

  constructor(
    private readonly gameService: GamesService,
    private readonly menuTabsService: MenuTabsService,
    private readonly authService: AuthService,
    private readonly dialog: MatDialog,
    private translate: TranslateService
  ) {}

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public ngOnInit(): void {
    let page: string = location.pathname;
    this.updateMenuItemsWithPage(page);
  }

  public openImagePreview(imageUrl: string) {
    this.dialog.open<ImagePreviewDialog, ImagePreviewData>(ImagePreviewDialog, {
      data: {
        imageUrl,
        title: this.translate.instant("image"),
      },
      width: '17rem',
    });
  }

  public openQrCodeDialog(): void {
    this.dialog.open(GameQrCodeDialog, {
      width: '17rem',
      data: {
        Id: this.game.id,
        Name: this.game.name,
        Type: QrCodeType.Game,
      },
    });
  }

  public updateMenuItemsWithPage(page: string) {
    this.menuItems = [
      {
        label: this.translate.instant('games.game.menu_items.info'),
        class: page == FULL_ROUTE.GAMES.DETAILS(this.game.id) ? 'active' : '',
        enabled: true,
        visible: true,
        route: FULL_ROUTE.GAMES.DETAILS(this.game.id),
      },
      {
        label: this.translate.instant('games.game.menu_items.availability'),
        class:
          page == FULL_ROUTE.GAMES.AVAILABILITY(this.game.id) ? 'active' : '',
        enabled: true,
        visible: true,
        route: FULL_ROUTE.GAMES.AVAILABILITY(this.game.id),
      },
      {
        label: this.translate.instant('games.game.menu_items.reviews'),
        class: page == FULL_ROUTE.GAMES.REVIEWS(this.game.id) ? 'active' : '',
        enabled: true,
        visible: true,
        route: FULL_ROUTE.GAMES.REVIEWS(this.game.id),
      },
    ];
  }

  public toggleGameLikeStatus(): void {
    const gameId = this.game.id;
    if (this.game.isLiked) {
      this.gameService.dislikeGame(gameId).subscribe((_) =>
        this.gameService.getById(gameId).subscribe((game) => {
          this.game = game;
          this.refresh.emit();
        })
      );
    } else {
      this.gameService.likeGame(gameId).subscribe((_) =>
        this.gameService.getById(gameId).subscribe((game) => {
          this.game = game;
          this.refresh.emit();
        })
      );
    }
  }
}
