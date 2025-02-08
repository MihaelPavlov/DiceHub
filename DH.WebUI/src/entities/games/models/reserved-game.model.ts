import { ReservationStatus } from '../../../shared/enums/reservation-status.enum';

export interface IReservedGame {
  id: number;
  reservationDate: Date;
  reservedDurationMinutes: number;
  isActive: boolean;
  isPaymentSuccessful: boolean;
  status: ReservationStatus;
  gameName: string;
  gameImageId: number;
  userName: string;
}
