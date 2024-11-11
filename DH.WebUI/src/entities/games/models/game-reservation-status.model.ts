export interface IGameReservationStatus {
  reservationId: number;
  gameId: number;
  reservationDate: Date;
  reservedDurationMinutes: number;
  isActive: boolean;
}
