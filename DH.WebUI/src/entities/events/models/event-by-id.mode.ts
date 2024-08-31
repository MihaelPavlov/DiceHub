export interface IEventByIdResult {
  id: number;
  name: string;
  description: string;
  startDate: Date;
  peopleJoined: number;
  maxPeople: number;
  isCustomImage: boolean;
  imageId: number;

  gameName: string;
  gameDescription: string;
  gameMinAge: number;
  gameMinPlayers: number;
  gameMaxPlayers: number;
  gameAveragePlaytime: number;
}
