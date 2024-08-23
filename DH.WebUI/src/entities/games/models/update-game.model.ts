export interface IUpdateGameDto {
  id: number;
  categoryId: number;
  name: string;
  description: string;
  minAge: number;
  minPlayers: number;
  maxPlayers: number;
  averagePlaytime: number;
  imageId?: number | null;
}
