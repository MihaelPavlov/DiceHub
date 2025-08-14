import { GameAveragePlaytime } from '../enums/game-average-playtime.enum';

export interface IUpdateGameDto {
  id: number;
  categoryId: number;
  name: string;
  description_EN: string;
  description_BG: string;
  minAge: number;
  minPlayers: number;
  maxPlayers: number;
  averagePlaytime: GameAveragePlaytime;
  imageId?: number | null;
}
