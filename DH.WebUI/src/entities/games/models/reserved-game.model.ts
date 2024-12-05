export interface IReservedGame {
  id: number;
  reservationDate: Date;
  reservedDurationMinutes: number;
  isActive: boolean;
  isPaymentSuccessful: boolean;
  gameName: string;
  gameImageId: number;
  userName: string;
}
