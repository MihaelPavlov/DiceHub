export interface ICreateGameDto {
  categoryId: number;
  name: string;
  description: string;
  minAge: number;
  minPlayers: number;
  maxPlayers: number;
  averagePlaytime: number;
}
