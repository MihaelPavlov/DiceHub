export interface IRoomListResult {
  id: number;
  userId: string;
  name: string;
  joinedParticipants: number;
  startDate: Date;
  maxParticipants: number;
  gameId: number;
  gameName: string;
  gameImageId: number;
  username: string;
}
