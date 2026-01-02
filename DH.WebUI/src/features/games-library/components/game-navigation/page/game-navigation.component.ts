import { TenantContextService } from './../../../../../shared/services/tenant-context.service';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GamesLibraryComponent } from '../../../../../pages/games-library/page/games-library.component';
import { GameCategoriesComponent } from '../../game-categories/page/game-categories.component';
import { NewGameListComponent } from '../../new-game-list/page/new-game-list.component';
import { IMenuItem } from '../../../../../shared/models/menu-item.model';
import { PermissionService } from '../../../../../shared/services/permission.service';
import { UserAction } from '../../../../../shared/constants/user-action';
import { GamesService } from '../../../../../entities/games/api/games.service';
import { IGameReservationStatus } from '../../../../../entities/games/models/game-reservation-status.model';
import { FULL_ROUTE } from '../../../../../shared/configs/route.config';
import { NavigationService } from '../../../../../shared/services/navigation-service';
import { TranslateService } from '@ngx-translate/core';
import { TenantRouter } from '../../../../../shared/helpers/tenant-router';

@Component({
  selector: 'app-game-navigation',
  templateUrl: 'game-navigation.component.html',
  styleUrl: 'game-navigation.component.scss',
  standalone: false,
})
export class GameNavigationComponent implements OnInit {
  private activeChildComponent!:
    | GamesLibraryComponent
    | GameCategoriesComponent
    | NewGameListComponent;

  public menuItems: BehaviorSubject<IMenuItem[]> = new BehaviorSubject<
    IMenuItem[]
  >([]);
  public isAdmin$: Observable<boolean> = this.permissionService.hasUserAction(
    UserAction.GamesCUD
  );

  public headerSectionName: string = 'Games';
  public gameReservationStatus: IGameReservationStatus | null = null;

  constructor(
    private readonly tenantRouter: TenantRouter,
    private readonly router: Router,
    private readonly permissionService: PermissionService,
    private readonly gameService: GamesService,
    private readonly navigationService: NavigationService,
    private readonly ts: TranslateService,
    private readonly tenantContextService: TenantContextService
  ) {
    this.handleMenuItemClick = this.handleMenuItemClick.bind(this);
  }

  public ngOnInit(): void {
    this.menuItems.next([
      { key: 'add-game', label: this.ts.instant('games.navigation.add_game') },
      {
        key: 'add-existing-game',
        label: this.ts.instant('games.navigation.add_existing_game'),
      },
    ]);

    this.gameService.userReservationStatus().subscribe({
      next: (status: IGameReservationStatus | null) => {
        this.gameReservationStatus = status;
      },
      error: () => {
        this.gameReservationStatus = null;
      },
    });
  }

  public handleMenuItemClick(key: string): void {
    if (key === 'add-game') {
      this.tenantRouter.navigateTenant(FULL_ROUTE.GAMES.ADD);
    } else if (key === 'add-existing-game') {
      this.tenantRouter.navigateTenant(FULL_ROUTE.GAMES.ADD_EXISTING_GAME);
    }
  }

  public onActivate(componentRef: any) {
    this.activeChildComponent = componentRef;

    if (
      this.activeChildComponent instanceof GamesLibraryComponent &&
      this.activeChildComponent.selectedCategoryName$
    ) {
      this.activeChildComponent.selectedCategoryName$.subscribe(
        (categoryName) => {
          if (categoryName) this.headerSectionName = categoryName;
          else this.headerSectionName = 'Games';
        }
      );
    }
  }

  public handleReservationWarningClick(): void {
    if (this.gameReservationStatus) {
      this.navigationService.setPreviousUrl(this.router.url);
      this.tenantRouter.navigateTenant(
        FULL_ROUTE.GAMES.AVAILABILITY(this.gameReservationStatus.gameId)
      );
    }
  }

  public handleSearchExpression(searchExpression: string) {
    if (
      this.activeChildComponent &&
      this.activeChildComponent.handleSearchExpression
    ) {
      this.activeChildComponent.handleSearchExpression(searchExpression);
    }
  }

  public getTenantLink(path: string): string {
    return `${this.tenantContextService.tenantId}/${path}`;
  }

  public isActiveLink(primaryLink: string): boolean {
    primaryLink = `/${this.tenantRouter.buildTenantUrl(primaryLink)}`;
    return this.router.url === primaryLink;
  }
}
