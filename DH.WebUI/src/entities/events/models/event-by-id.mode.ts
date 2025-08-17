export interface IEventByIdResult {
  id: number;
  name: string;
  description_EN: string;
  description_BG: string;
  startDate: Date;
  peopleJoined: number;
  maxPeople: number;
  isCustomImage: boolean;
  imageId: number;

  gameId: number;
  gameName: string;
  gameDescription_EN: string;
  gameDescription_BG: string;
  gameMinAge: number;
  gameMinPlayers: number;
  gameMaxPlayers: number;
  gameAveragePlaytime: number;
}
