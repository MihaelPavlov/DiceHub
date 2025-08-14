import { GameAveragePlaytime } from '../enums/game-average-playtime.enum';

export interface ICreateGameDto {
  categoryId: number;
  name: string;
  description_EN: string;
  description_BG: string;
  minAge: number;
  minPlayers: number;
  maxPlayers: number;
  averagePlaytime: GameAveragePlaytime;
}
