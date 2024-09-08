export interface IAddUpdateRoomDto {
  id?: number;
  name: string;
  startDate: string;
  maxParticipants: number;
  gameId: number;
}
