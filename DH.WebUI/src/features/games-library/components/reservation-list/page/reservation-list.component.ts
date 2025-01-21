import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GamesService } from '../../../../../entities/games/api/games.service';
import { IReservedGame } from '../../../../../entities/games/models/reserved-game.model';
import { Observable } from 'rxjs';
import { ImageEntityType } from '../../../../../shared/pipe/entity-image.pipe';

@Component({
  selector: 'app-game-reservation-list',
  templateUrl: 'reservation-list.component.html',
  styleUrl: 'reservation-list.component.scss',
})
export class ReservationListComponent implements OnInit {
  public reservedGames$!: Observable<IReservedGame[]>;
  public readonly ImageEntityType = ImageEntityType;

  constructor(
    private readonly router: Router,
    private readonly gameService: GamesService
  ) {}

  public ngOnInit(): void {
    this.reservedGames$ = this.gameService.getReservations();
  }

  public backNavigateBtn() {
    this.router.navigateByUrl('games/library');
  }
}
