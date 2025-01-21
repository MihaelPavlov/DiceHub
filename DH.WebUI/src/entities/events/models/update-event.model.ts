export interface IUpdateEventDto {
  id: number;
  gameId: number;
  name: string;
  description: string;
  startDate: string;
  maxPeople: number;
  isCustomImage: boolean;
}
