import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { GamesService } from '../../../entities/games/api/games.service';
import { IGameListResult } from '../../../entities/games/models/game-list.model';
import { MenuTabsService } from '../../../shared/services/menu-tabs.service';
import { NAV_ITEM_LABELS } from '../../../shared/models/nav-items-labels.const';
import { SearchService } from '../../../shared/services/search.service';
import { IMenuItem } from '../../../shared/models/menu-item.model';
import { PermissionService } from '../../../shared/services/permission.service';
import { UserAction } from '../../../shared/constants/user-action';
import { GameConfirmDeleteDialog } from '../../../features/games-library/dialogs/game-confirm-delete-dialog/game-confirm-delete.component';
import { MatDialog } from '@angular/material/dialog';
import { GameQrCodeDialog } from '../../../features/games-library/dialogs/qr-code-dialog/qr-code-dialog.component';
import { BehaviorSubject, Observable } from 'rxjs';
import { ControlsMenuComponent } from '../../../shared/components/menu/controls-menu.component';

@Component({
  selector: 'app-games-library',
  templateUrl: 'games-library.component.html',
  styleUrl: 'games-library.component.scss',
})
export class GamesLibraryComponent implements OnInit, OnDestroy {
  public games: IGameListResult[] = [];
  public menuItems: BehaviorSubject<IMenuItem[]> = new BehaviorSubject<
    IMenuItem[]
  >([]);
  private categoryId: number | null = null;
  public visibleMenuId: number | null = null;
  public isAdmin$: Observable<boolean> = this.permissionService.hasUserAction(
    UserAction.GamesCUD
  );

  constructor(
    private readonly router: Router,
    private readonly activeRoute: ActivatedRoute,
    private readonly gameService: GamesService,
    private readonly menuTabsService: MenuTabsService,
    private readonly searchService: SearchService,
    private readonly permissionService: PermissionService,
    private readonly dialog: MatDialog
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.GAMES);
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
    this.searchService.hideSearchForm();
  }

  public showMenu(
    gameId: number,
    event: MouseEvent,
    controlMenu: ControlsMenuComponent
  ): void {
    event.stopPropagation();
    this.visibleMenuId = this.visibleMenuId === gameId ? null : gameId;
    controlMenu.toggleMenu();
  }

  public ngOnInit(): void {
    this.menuItems.next([
      { key: 'qr-code', label: 'QR Code' },
      { key: 'history-log', label: 'Last Activities' },
      { key: 'update', label: 'Update' },
      { key: 'copy', label: 'Add Copy' },
      { key: 'delete', label: 'Delete' },
    ]);

    this.activeRoute.params.subscribe((params: Params) => {
      this.categoryId = params['id'];

      if (this.categoryId) {
        this.fetchGameListByCategoryId(this.categoryId);
      }
    });

    if (!this.categoryId) {
      this.fetchGameList();
    }
  }

  public navigateToGameDetails(id: number): void {
    this.router.navigateByUrl(`games/${id}/details`);
  }

  public handleSearchExpression(searchExpression: string) {
    if (this.categoryId) {
      this.fetchGameListByCategoryId(this.categoryId, searchExpression);
    } else {
      this.fetchGameList(searchExpression);
    }
  }

  public onMenuOption(key: string, event: MouseEvent): void {
    event.stopPropagation();
    if (key === 'update') {
      this.router.navigateByUrl(`games/${this.visibleMenuId}/update`);
    } else if (key === 'copy') {
      this.router.navigateByUrl(
        `games/${this.visibleMenuId}/add-existing-game`
      );
    } else if (key === 'delete' && this.visibleMenuId) {
      this.openDeleteDialog(this.visibleMenuId);
    } else if (key === 'qr-code') {
      this.dialog.open(GameQrCodeDialog, {
        width: '17rem',
        data: { id: this.visibleMenuId },
      });
    }
    this.visibleMenuId = null;
  }

  private fetchGameListByCategoryId(id: number, searchExpression: string = '') {
    this.gameService
      .getListByCategoryId(id, searchExpression)
      .subscribe((gameList) => (this.games = gameList ?? []));
  }

  private fetchGameList(searchExpression: string = '') {
    this.gameService.getList(searchExpression).subscribe({
      next: (gameList) => (this.games = gameList ?? []),
      error: (error) => {
        console.log(error);
      },
    });
  }

  private openDeleteDialog(id: number): void {
    const dialogRef = this.dialog.open(GameConfirmDeleteDialog, {
      data: { id: id },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.fetchGameList();
      }
    });
  }
}
