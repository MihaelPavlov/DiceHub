export interface IReservedGame {
  id: number;
  reservationDate: Date;
  reservedDurationMinutes: number;
  isExpired: boolean;
  isPaymentSuccessful: boolean;
  gameName: string;
  gameImageId: number;
  userName: string;
}
