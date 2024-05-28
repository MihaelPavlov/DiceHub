import { Component, Input } from '@angular/core';
import { IGameByIdResult } from '../../../../../entities/games/models/game-by-id.model';

@Component({
  selector: 'app-game-layout',
  templateUrl: 'game-layout.component.html',
  styleUrl: 'game-layout.component.scss',
})
export class GameLayoutComponent {
  @Input() game!: IGameByIdResult;
}
