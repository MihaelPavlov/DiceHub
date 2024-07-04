import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Params, Router } from "@angular/router";
import { GamesService } from "../../../../../entities/games/api/games.service";
import { Observable } from "rxjs";
import { IGameByIdResult } from "../../../../../entities/games/models/game-by-id.model";

@Component({
    selector: 'app-game-availability',
    templateUrl: 'game-availability.component.html',
    styleUrl: 'game-availability.component.scss'
  })
  export class GameAvailabilityComponent implements OnInit {
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