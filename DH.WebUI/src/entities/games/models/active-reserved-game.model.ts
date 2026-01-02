import { ReservationStatus } from '../../../shared/enums/reservation-status.enum';

export interface ActiveReservedGame {
  id: number;
  gameId: number;
  gameName: string;
  gameImageUrl: string;
  reservationDate: Date;
  reservedDurationMinutes: number;
  status: ReservationStatus;
  numberOfGuests: number;
  createdDate: Date;
  username: string;
  phoneNumber: string;
  userLanguage: string;
  userId: string;
  userHaveActiveTableReservation: boolean;
  tableReservationTime?: Date | null;
}
