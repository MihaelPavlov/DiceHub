export interface IGameReservationStatus {
  gameId: number;
  reservationDate: Date;
  reservedDurationMinutes: number;
  isExpired: boolean;
}
