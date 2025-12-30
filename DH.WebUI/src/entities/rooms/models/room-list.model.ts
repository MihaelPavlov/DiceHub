export interface IRoomListResult {
  id: number;
  userId: string;
  name: string;
  joinedParticipants: number;
  startDate: Date;
  maxParticipants: number;
  gameId: number;
  gameName: string;
  gameImageUrl: string;
  username: string;
}
