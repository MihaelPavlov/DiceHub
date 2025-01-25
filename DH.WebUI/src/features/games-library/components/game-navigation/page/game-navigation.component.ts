import { BehaviorSubject, Observable, of } from 'rxjs';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GamesLibraryComponent } from '../../../../../pages/games-library/page/games-library.component';
import { GameCategoriesComponent } from '../../game-categories/page/game-categories.component';
import { NewGameListComponent } from '../../new-game-list/page/new-game-list.component';
import { IMenuItem } from '../../../../../shared/models/menu-item.model';
import { PermissionService } from '../../../../../shared/services/permission.service';
import { UserAction } from '../../../../../shared/constants/user-action';

@Component({
  selector: 'app-game-navigation',
  templateUrl: 'game-navigation.component.html',
  styleUrl: 'game-navigation.component.scss',
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
  constructor(
    private readonly router: Router,
    private readonly permissionService: PermissionService
  ) {
    this.handleMenuItemClick = this.handleMenuItemClick.bind(this);
  }

  public ngOnInit(): void {
    this.menuItems.next([
      { key: 'add-game', label: 'Add Game' },
      { key: 'add-existing-game', label: 'Add Existing Game' },
      { key: 'reserved-games', label: 'Reserved Games' },
    ]);
  }

  public handleMenuItemClick(key: string): void {
    if (key === 'add-game') {
      this.router.navigateByUrl('/games/add');
    } else if (key === 'add-existing-game') {
      this.router.navigateByUrl('/games/add-existing-game');
    } else if (key === 'reserved-games') {
      this.router.navigateByUrl('/games/reservations');
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
          console.log(categoryName);

          if (categoryName) this.headerSectionName = categoryName;
          else this.headerSectionName = 'Games';
        }
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

  public isActiveLink(
    primaryLink: string,
    secondaryLink: string | null = null,
    strictMatch: boolean = false
  ): boolean {
    if (strictMatch) {
      return this.router.url === primaryLink;
    }

    return (
      this.router.url.includes(primaryLink) ||
      (secondaryLink !== null && this.router.url.includes(secondaryLink))
    );
  }
}
