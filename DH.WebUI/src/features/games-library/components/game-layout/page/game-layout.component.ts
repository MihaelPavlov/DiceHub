import {
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
} from '@angular/core';
import { IGameByIdResult } from '../../../../../entities/games/models/game-by-id.model';
import { StringFormatPipe } from '../../../../../shared/pipe/string-format.pipe';
import { ROUTES } from '../../../../../shared/configs/routes.config';
import { GamesService } from '../../../../../entities/games/api/games.service';
import { MenuTabsService } from '../../../../../shared/services/menu-tabs.service';
import { NavItemInterface } from '../../../../../shared/models/nav-item.mode';
import { MatDialog } from '@angular/material/dialog';
import { GameQrCodeDialog } from '../../../dialogs/qr-code-dialog/qr-code-dialog.component';
import { AuthService } from '../../../../../entities/auth/auth.service';
import { UserRole } from '../../../../../entities/auth/enums/roles.enum';

@Component({
  selector: 'app-game-layout',
  templateUrl: 'game-layout.component.html',
  styleUrl: 'game-layout.component.scss',
})
export class GameLayoutComponent implements OnInit, OnDestroy {
  @Input() game!: IGameByIdResult;
  @Input() backNavigateBtn: () => void = () => {};
  @Output() refresh = new EventEmitter<void>();

  public isQrCodeVisible: boolean = this.authService.getUser?.role !== UserRole.User;
  public menuItems: NavItemInterface[] = [];

  constructor(
    private readonly stringFormat: StringFormatPipe,
    private readonly gameService: GamesService,
    private readonly menuTabsService: MenuTabsService,
    private readonly authService: AuthService,
    private readonly dialog: MatDialog
  ) {}

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public ngOnInit(): void {
    let page: string = location.pathname;
    this.updateMenuItemsWihtPage(page);
  }

  public openQrCodeDialog(): void {
    this.dialog.open(GameQrCodeDialog, {
      width: '17rem',
      position: { bottom: '60%', left: '2%' },
      data: { id: this.game.id },
    });
  }

  public updateMenuItemsWihtPage(page: string) {
    this.menuItems = [
      {
        label: 'Statistics',
        class:
          page ==
          this.stringFormat.transform(ROUTES.GAMES.DETAILS, {
            id: this.game.id,
          })
            ? 'active'
            : '',
        enabled: true,
        visible: true,
        route: this.stringFormat.transform(ROUTES.GAMES.DETAILS, {
          id: this.game.id,
        }),
      },
      {
        label: 'Availability',
        class:
          page ==
          this.stringFormat.transform(ROUTES.GAMES.DETAILS_AVAILABILITY, {
            id: this.game.id,
          })
            ? 'active'
            : '',
        enabled: true,
        visible: true,
        route: this.stringFormat.transform(ROUTES.GAMES.DETAILS_AVAILABILITY, {
          id: this.game.id,
        }),
      },
      {
        label: 'Reviews',
        class:
          page ==
          this.stringFormat.transform(ROUTES.GAMES.DETAILS_REVIEWS, {
            id: this.game.id,
          })
            ? 'active'
            : '',
        enabled: true,
        visible: true,
        route: this.stringFormat.transform(ROUTES.GAMES.DETAILS_REVIEWS, {
          id: this.game.id,
        }),
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
