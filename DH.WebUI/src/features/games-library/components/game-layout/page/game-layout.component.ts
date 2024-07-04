import { Component, Input, OnInit } from '@angular/core';
import { IGameByIdResult } from '../../../../../entities/games/models/game-by-id.model';

export interface MenuItemInterface {
  label: string;
  class: string;
  enabled: boolean;
  visible: boolean;
  route: string;
  translatable?: boolean;
}

@Component({
  selector: 'app-game-layout',
  templateUrl: 'game-layout.component.html',
  styleUrl: 'game-layout.component.scss',
})
export class GameLayoutComponent implements OnInit {
  @Input() game!: IGameByIdResult;
  @Input() backNavigateBtn: () => void = () => {};
  public menuItems: MenuItemInterface[] = [];

  public ngOnInit(): void {
    let page: string = location.pathname;

    this.updateMenuItemsWihtPage(page);
  }

  public updateMenuItemsWihtPage(page: string) {
    this.menuItems = [
      {
        label: 'Statistics',
        class: page == `/games/${this.game.id}/details` ? 'active' : '',
        enabled: true,
        visible: true,
        route: `/games/${this.game.id}/details`,
      },
      {
        label: 'Availability',
        class: page == `/games/${this.game.id}/availability` ? 'active' : '',
        enabled: true,
        visible: true,
        route: `/games/${this.game.id}/availability`,
      },
      {
        label: 'Reviews',
        class: page == `/games/${this.game.id}/reviews` ? 'active' : '',
        enabled: true,
        visible: true,
        route: `/games/${this.game.id}/reviews`,
      },
    ];
  }
}
