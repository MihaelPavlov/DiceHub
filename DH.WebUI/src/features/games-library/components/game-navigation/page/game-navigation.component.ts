import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { GamesLibraryComponent } from '../../../../../pages/games-library/page/games-library.component';
import { GameCategoriesComponent } from '../../game-categories/page/game-categories.component';

@Component({
  selector: 'app-game-navigation',
  templateUrl: 'game-navigation.component.html',
  styleUrl: 'game-navigation.component.scss',
})
export class GameNavigationComponent {
  private activeChildComponent!:
    | GamesLibraryComponent
    | GameCategoriesComponent;

  constructor(private router: Router) {}

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
    return this.router.url === link;
  }
}
