export interface IGameByIdResult {
  id: number;
  categoryId: number;
  name: string;
  description: string;
  likes: number;
  isLiked: boolean;
  imageId: number;
  minAge: number;
  minPlayers: number;
  maxPlayers: number;
  averagePlaytime: number;
}
