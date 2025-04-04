import { ReservationStatus } from '../../../shared/enums/reservation-status.enum';

export interface IGetReservationById {
  id: number;
  userId: string;
  username: string;
  numberOfGuests: number;
  reservationDate: Date;
  createdDate: Date;
  isActive: boolean;
  status: ReservationStatus;
  isReservationSuccessful: boolean;
  internalNote: string;
  publicNote: string;
}
