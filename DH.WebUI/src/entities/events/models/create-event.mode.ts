export interface ICreateEventDto {
  gameId: number;
  name: string;
  description: string;
  startDate: Date;
  maxPeople: number;
  isCustomImage: boolean;
}
