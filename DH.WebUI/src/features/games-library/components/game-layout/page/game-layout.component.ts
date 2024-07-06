import { Component, Input, OnInit } from '@angular/core';
import { IGameByIdResult } from '../../../../../entities/games/models/game-by-id.model';
import { StringFormatPipe } from '../../../../../shared/pipe/string-format.pipe';
import { ROUTES } from '../../../../../shared/configs/routes.config';

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

  constructor(private readonly stringFormat: StringFormatPipe) {}

  public ngOnInit(): void {
    let page: string = location.pathname;
    this.updateMenuItemsWihtPage(page);
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
}
