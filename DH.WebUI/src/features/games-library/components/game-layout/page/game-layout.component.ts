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

@Component({
  selector: 'app-game-layout',
  templateUrl: 'game-layout.component.html',
  styleUrl: 'game-layout.component.scss',
})
export class GameLayoutComponent implements OnInit, OnDestroy {
  @Input() game!: IGameByIdResult;
  @Input() backNavigateBtn: () => void = () => {};
  @Output() refresh = new EventEmitter<void>();

  public menuItems: NavItemInterface[] = [];

  constructor(
    private readonly stringFormat: StringFormatPipe,
    private readonly gameService: GamesService,
    private readonly menuTabsService: MenuTabsService
  ) {}

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

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
