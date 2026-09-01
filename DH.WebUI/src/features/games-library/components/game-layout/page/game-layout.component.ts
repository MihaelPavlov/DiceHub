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
import { QrCodeDialog } from '../../../dialogs/qr-code-dialog/qr-code-dialog.component';
import { GameConfirmDeleteDialog } from '../../../dialogs/game-confirm-delete-dialog/game-confirm-delete.component';
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
import { TenantRouter } from '../../../../../shared/helpers/tenant-router';
import { BehaviorSubject, Observable } from 'rxjs';
import { IMenuItem } from '../../../../../shared/models/menu-item.model';
import { PermissionService } from '../../../../../shared/services/permission.service';
import { UserAction } from '../../../../../shared/constants/user-action';
import { ControlsMenuComponent } from '../../../../../shared/components/menu/controls-menu.component';

@Component({
    selector: 'app-game-layout',
    templateUrl: 'game-layout.component.html',
    styleUrl: 'game-layout.component.scss',
    standalone: false
})
export class GameLayoutComponent implements OnInit, OnDestroy {
  @Input() game!: IGameByIdResult;
  @Input() backNavigateBtn: () => void = () => {};
  @Output() refresh = new EventEmitter<void>();

  public isQrCodeVisible: boolean =
    this.authService.getUser?.role !== UserRole.User;
  public menuItems: NavItemInterface[] = [];
  public readonly ImageEntityType = ImageEntityType;

  // Same game-actions menu as the games library cards, reachable from any game
  // sub-page (info / availability / reviews).
  public isAdmin$: Observable<boolean> = this.permissionService.hasUserAction(
    UserAction.GamesCUD
  );
  public gameMenuItems: BehaviorSubject<IMenuItem[]> = new BehaviorSubject<
    IMenuItem[]
  >([]);

  constructor(
    private readonly gameService: GamesService,
    private readonly menuTabsService: MenuTabsService,
    private readonly authService: AuthService,
    private readonly dialog: MatDialog,
    private translate: TranslateService,
    private readonly tenantRouter: TenantRouter,
    private readonly permissionService: PermissionService
  ) {}

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public ngOnInit(): void {
    let page: string = location.pathname;
    this.updateMenuItemsWithPage(page);

    this.gameMenuItems.next([
      {
        key: 'qr-code',
        label: this.translate.instant('games.library.menu_items.qr_code'),
      },
      {
        key: 'update',
        label: this.translate.instant('games.library.menu_items.update'),
      },
      {
        key: 'copy',
        label: this.translate.instant('games.library.menu_items.add_copy'),
      },
      {
        key: 'delete',
        label: this.translate.instant('games.library.menu_items.delete'),
      },
    ]);
  }

  public showGameMenu(event: MouseEvent, menu: ControlsMenuComponent): void {
    event.stopPropagation();
    menu.toggleMenu();
  }

  public onGameMenuOption(key: string, event: MouseEvent): void {
    event.stopPropagation();

    if (key === 'qr-code') {
      this.openQrCodeDialog();
    } else if (key === 'update') {
      this.tenantRouter.navigateTenant(FULL_ROUTE.GAMES.UPDATE(this.game.id));
    } else if (key === 'copy') {
      this.tenantRouter.navigateTenant(
        FULL_ROUTE.GAMES.ADD_EXISTING_GAME_BY_ID(this.game.id)
      );
    } else if (key === 'delete') {
      this.dialog
        .open(GameConfirmDeleteDialog, {
          panelClass: 'confirm-sheet-pane',
          data: { id: this.game.id },
        })
        .afterClosed()
        .subscribe((result) => {
          if (result) {
            this.tenantRouter.navigateTenant(FULL_ROUTE.GAMES.LIBRARY);
          }
        });
    }
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
    this.dialog.open(QrCodeDialog, {
      width: '19rem',
      data: {
        Id: this.game.id,
        Name: this.game.name,
        Type: QrCodeType.Game,
      },
    });
  }

  public updateMenuItemsWithPage(page: string) {
    const detailsRoute = FULL_ROUTE.GAMES.DETAILS(this.game.id);
    const availabilityRoute = FULL_ROUTE.GAMES.AVAILABILITY(this.game.id);
    const reviewsRoute = FULL_ROUTE.GAMES.REVIEWS(this.game.id);

    this.menuItems = [
      {
        label: this.translate.instant('games.game.menu_items.info'),
        class: page.endsWith(detailsRoute) ? 'active' : '',
        enabled: true,
        visible: true,
        route: this.buildTenantRoute(detailsRoute),
      },
      {
        label: this.translate.instant('games.game.menu_items.availability'),
        class: page.endsWith(availabilityRoute) ? 'active' : '',
        enabled: true,
        visible: true,
        route: this.buildTenantRoute(availabilityRoute),
      },
      {
        label: this.translate.instant('games.game.menu_items.reviews'),
        class: page.endsWith(reviewsRoute) ? 'active' : '',
        enabled: true,
        visible: true,
        route: this.buildTenantRoute(reviewsRoute),
      },
    ];
  }

  private buildTenantRoute(route: string): string {
    return `/${this.tenantRouter.buildTenantUrl(route)}`;
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
