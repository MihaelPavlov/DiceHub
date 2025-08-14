import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { GamesService } from '../../../../../entities/games/api/games.service';
import { Observable } from 'rxjs';
import { IGameByIdResult } from '../../../../../entities/games/models/game-by-id.model';
import { MenuTabsService } from '../../../../../shared/services/menu-tabs.service';
import { NAV_ITEM_LABELS } from '../../../../../shared/models/nav-items-labels.const';
import { NavigationService } from '../../../../../shared/services/navigation-service';
import { SupportLanguages } from '../../../../../entities/common/models/support-languages.enum';
import { LanguageService } from '../../../../../shared/services/language.service';

@Component({
  selector: 'app-game-details',
  templateUrl: 'game-details.component.html',
  styleUrl: 'game-details.component.scss',
})
export class GameDetailsComponent implements OnInit, OnDestroy {
  public game$!: Observable<IGameByIdResult>;
  private gameId!: number;
  public readonly SupportLanguages = SupportLanguages;

  constructor(
    private readonly gameService: GamesService,
    private readonly activeRoute: ActivatedRoute,
    private readonly menuTabsService: MenuTabsService,
    private readonly router: Router,
    private readonly navigationService: NavigationService,
    private readonly languageService: LanguageService
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.GAMES);
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public get currentLanguage(): SupportLanguages {    
    return this.languageService.getCurrentLanguage();
  }

  public ngOnInit(): void {
    this.activeRoute.params.subscribe((params: Params) => {
      this.gameId = params['id'];
      this.fetchGame();
    });
  }

  public navigateBack(): void {
    this.router.navigateByUrl(
      this.navigationService.getPreviousUrl() ?? '/games/library'
    );
  }

  public fetchGame(): void {
    this.game$ = this.gameService.getById(this.gameId);
  }
}
