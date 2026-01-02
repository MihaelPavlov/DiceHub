export interface IRoomByIdResult {
  id: number;
  createdBy: string;
  name: string;
  joinedParticipants: number;
  startDate: Date;
  maxParticipants: number;
  gameId: number;
  gameImageUrl: string;
  username: string;
}
