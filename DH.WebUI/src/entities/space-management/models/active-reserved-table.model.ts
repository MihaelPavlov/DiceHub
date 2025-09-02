import { ReservationStatus } from '../../../shared/enums/reservation-status.enum';

export interface IActiveReservedTable {
  id: number;
  userId: string;
  username: string;
  phoneNumber: string;
  userLanguage: string;
  numberOfGuests: number;
  reservationDate: Date;
  status: ReservationStatus;
  createdDate: Date;
  isActive: boolean;
}
