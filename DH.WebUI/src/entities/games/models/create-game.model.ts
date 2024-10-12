import { GameAveragePlaytime } from "../enums/game-average-playtime.enum";

export interface ICreateGameDto {
  categoryId: number;
  name: string;
  description: string;
  minAge: number;
  minPlayers: number;
  maxPlayers: number;
  averagePlaytime: GameAveragePlaytime;
}
