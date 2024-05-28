import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { GamesService } from '../../../../../entities/games/api/games.service';
import { Observable } from 'rxjs';
import { IGameByIdResult } from '../../../../../entities/games/models/game-by-id.model';

@Component({
  selector: 'app-game-details',
  templateUrl: 'game-details.component.html',
  styleUrl: 'game-details.component.scss',
})
export class GameDetailsComponent implements OnInit {
  game$!: Observable<IGameByIdResult>;

  constructor(
    private readonly gameService: GamesService,
    private readonly route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe((params: Params) => {
      const gameId = params['id'];
      this.game$ = this.gameService.getById(gameId);
    });
  }
}
