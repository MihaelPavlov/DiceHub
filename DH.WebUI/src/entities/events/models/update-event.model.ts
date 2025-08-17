export interface IUpdateEventDto {
  id: number;
  gameId: number;
  name: string;
  description_EN: string;
  description_BG: string;
  startDate: string;
  maxPeople: number;
  isCustomImage: boolean;
}
