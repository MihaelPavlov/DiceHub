export interface ICreateEventDto {
  gameId: number;
  name: string;
  description: string;
  startDate: string;
  maxPeople: number;
  isCustomImage: boolean;
}
