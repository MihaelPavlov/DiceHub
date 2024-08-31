export interface IUpdateEventDto {
  id: number;
  gameId: number;
  name: string;
  description: string;
  startDate: Date;
  maxPeople: number;
  isCustomImage: boolean;
}
