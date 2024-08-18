export interface IGameByIdResult {
  id: number;
  name: string;
  description: string;
  likes: number;
  isLiked: boolean;
  imageUrl: string;
  minAge: number;
  minPlayers: number;
  maxPlayers: number;
  averagePlaytime: number;
}
