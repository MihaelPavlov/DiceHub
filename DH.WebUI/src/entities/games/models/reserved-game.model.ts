export interface IReservedGame {
  reservationDate: Date;
  reservedDurationMinutes: number;
  isExpired: boolean;
  isPaymentSuccessful: boolean;
  gameName: string;
  gameImageId: number;
  userName: string;
}
