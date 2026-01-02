import { GameAveragePlaytime } from '../enums/game-average-playtime.enum';

export interface IGameByIdResult {
  id: number;
  categoryId: number;
  name: string;
  description_EN: string;
  description_BG: string;
  likes: number;
  isLiked: boolean;
  imageUrl: string;
  minAge: number;
  minPlayers: number;
  maxPlayers: number;
  averagePlaytime: GameAveragePlaytime;
}
