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

  public menuItems: IMenuItem[] = [];
  public isAdmin = this.permissionService.hasUserAction(UserAction.GamesCUD)
  constructor(
    private readonly router: Router,
    private readonly permissionService: PermissionService
  ) {}

  public ngOnInit(): void {
    this.menuItems = [
      { key: 'add-user', label: 'Add User' },
      { key: 'add-existing-user', label: 'Add Existing User' },
      { key: 'reserved-games', label: 'Reserved Games' },
    ];
  }

  public handleMenuItemClick(key: string): void {
    if (key === 'add-user') {
      //navigate to add user
    } else if (key === 'add-existing-user') {
      //navigate to
    } else if (key === 'reserved-games') {
      //navigate to
    }
    console.log(`Selected: ${key}`);
  }

  public onActivate(componentRef: any) {
    this.activeChildComponent = componentRef;
  }

  public handleSeachExpression(searchExpression: string) {
    if (
      this.activeChildComponent &&
      this.activeChildComponent.handleSearchExpression
    ) {
      this.activeChildComponent.handleSearchExpression(searchExpression);
    }
  }

  public isActiveLink(link: string): boolean {
    return this.router.url.includes(link);
  }
}
