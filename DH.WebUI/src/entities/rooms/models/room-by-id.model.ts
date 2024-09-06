export interface IRoomByIdResult {
  id: number;
  userId: string;
  name: string;
  joinedParticipants: number;
  startDate: Date;
  maxParticipants: number;
  gameId: number;
  gameImageId: number;
  username: string;
}
