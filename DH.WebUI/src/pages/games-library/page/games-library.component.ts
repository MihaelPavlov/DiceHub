import { TranslateService } from '@ngx-translate/core';
import { SupportLanguages } from './../../../entities/common/models/support-languages.enum';
import { NavigationService } from './../../../shared/services/navigation-service';
import { IGameCategory } from './../../../entities/games/models/game-category.model';
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
import { QrCodeDialog } from '../../../features/games-library/dialogs/qr-code-dialog/qr-code-dialog.component';
import { BehaviorSubject, combineLatest, Observable } from 'rxjs';
import { ControlsMenuComponent } from '../../../shared/components/menu/controls-menu.component';
import { QrCodeType } from '../../../entities/qr-code-scanner/enums/qr-code-type.enum';
import { ImageEntityType } from '../../../shared/pipe/entity-image.pipe';
import { GameCategoriesService } from '../../../entities/games/api/game-categories.service';
import { FULL_ROUTE } from '../../../shared/configs/route.config';
import { LanguageService } from '../../../shared/services/language.service';

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
  public readonly ImageEntityType = ImageEntityType;
  public readonly SupportLanguages = SupportLanguages;
  public selectedCategoryName$ = new BehaviorSubject<string | null>(null);
  public categoryList: IGameCategory[] = [];
  constructor(
    private readonly router: Router,
    private readonly activeRoute: ActivatedRoute,
    private readonly gameService: GamesService,
    private readonly menuTabsService: MenuTabsService,
    private readonly searchService: SearchService,
    private readonly permissionService: PermissionService,
    private readonly dialog: MatDialog,
    private readonly gameCategoriesService: GameCategoriesService,
    private readonly navigationService: NavigationService,
    private readonly languageService: LanguageService,
    private readonly translateService: TranslateService
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.GAMES);
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
    this.searchService.hideSearchForm();

    this.selectedCategoryName$.next(null);
  }

  public get currentLanguage(): SupportLanguages {
    return this.languageService.getCurrentLanguage();
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
      {
        key: 'qr-code',
        label: this.translateService.instant(
          'games.library.menu_items.qr_code'
        ),
      },
      {
        key: 'update',
        label: this.translateService.instant('games.library.menu_items.update'),
      },
      {
        key: 'copy',
        label: this.translateService.instant(
          'games.library.menu_items.add_copy'
        ),
      },
      {
        key: 'delete',
        label: this.translateService.instant('games.library.menu_items.delete'),
      },
    ]);

    this.activeRoute.params.subscribe((params: Params) => {
      this.categoryId = params['id'];

      if (this.categoryId) {
        this.fetchGameListByCategoryId(this.categoryId, '', true);
      }
    });

    if (!this.categoryId) {
      this.fetchGameList();
    }
  }

  public navigateToGameDetails(id: number): void {
    this.navigationService.setPreviousUrl(this.router.url);
    this.router.navigateByUrl(FULL_ROUTE.GAMES.DETAILS(id));
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
    if (key === 'update' && this.visibleMenuId) {
      this.router.navigateByUrl(FULL_ROUTE.GAMES.UPDATE(this.visibleMenuId));
    } else if (key === 'copy' && this.visibleMenuId) {
      this.router.navigateByUrl(
        FULL_ROUTE.GAMES.ADD_EXISTING_GAME(this.visibleMenuId)
      );
    } else if (key === 'delete' && this.visibleMenuId) {
      this.openDeleteDialog(this.visibleMenuId);
    } else if (key === 'qr-code') {
      this.dialog.open(QrCodeDialog, {
        width: '17rem',
        data: {
          Id: this.visibleMenuId,
          Name:
            this.games.find((x) => x.id === this.visibleMenuId)?.name ??
            this.translateService.instant(
              'games.library.missing_name_for_qr_code'
            ),
          Type: QrCodeType.Game,
        },
      });
    }
    this.visibleMenuId = null;
  }

  private fetchGameListByCategoryId(
    id: number,
    searchExpression: string = '',
    withCategories: boolean = false
  ) {
    if (withCategories) {
      combineLatest([
        this.gameService.getListByCategoryId(id, searchExpression),
        this.gameCategoriesService.getList(''),
      ]).subscribe(
        ([gameList, categories]: [
          IGameListResult[] | null,
          IGameCategory[] | null
        ]) => {
          this.games = gameList ?? [];
          this.categoryList = categories ?? [];

          const categoryName = this.categoryList.find(
            (x) => x.id === Number(id)
          );

          this.selectedCategoryName$.next(categoryName?.name ?? null);
        }
      );
    } else {
      this.gameService
        .getListByCategoryId(id, searchExpression)
        .subscribe((gameList) => {
          this.games = gameList ?? [];
        });
    }
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
