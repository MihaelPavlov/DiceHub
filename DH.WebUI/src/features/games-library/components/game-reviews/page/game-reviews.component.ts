import { Component, OnInit } from '@angular/core';
import { IGameByIdResult } from '../../../../../entities/games/models/game-by-id.model';
import { Observable } from 'rxjs';
import { GamesService } from '../../../../../entities/games/api/games.service';
import { ActivatedRoute, Params, Router } from '@angular/router';

@Component({
  selector: 'app-game-reviews',
  templateUrl: 'game-reviews.component.html',
  styleUrl: 'game-reviews.component.scss',
})
export class GameReviewsComponent implements OnInit {
  public game$!: Observable<IGameByIdResult>;

  constructor(
    private readonly gameService: GamesService,
    private readonly activeRoute: ActivatedRoute,
    private readonly router: Router
  ) {}

  public ngOnInit(): void {
    this.activeRoute.params.subscribe((params: Params) => {
      const gameId = params['id'];
      this.game$ = this.gameService.getById(gameId);
    });
  }

  public navigateBackToGameList(): void {
    this.router.navigate(['games/library']);
  }
}
