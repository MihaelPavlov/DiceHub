import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GamesService } from '../../../entities/games/api/games.service';
import { Observable } from 'rxjs';
import { IGameListResult } from '../../../entities/games/models/game-list.model';

@Component({
  selector: 'app-games-library',
  templateUrl: 'games-library.component.html',
  styleUrl: 'games-library.component.scss',
})
export class GamesLibraryComponent implements OnInit {
  readonly games$: Observable<IGameListResult[]> = this.gameService.getList();

  constructor(
    private readonly router: Router,
    private readonly gameService: GamesService
  ) {}

  ngOnInit(): void {

  }

  public navigateToGameDetails(id:number): void {
    this.router.navigateByUrl(`games/${id}/details`);
  }
}
