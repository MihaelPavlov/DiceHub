import { ReservationStatus } from '../../../shared/enums/reservation-status.enum';

export interface ActiveReservedGame {
  id: number;
  gameId: number;
  gameName: string;
  gameImageId: number;
  reservationDate: Date;
  reservedDurationMinutes: number;
  status: ReservationStatus;
  numberOfGuests: number;
  createdDate: Date;
  username: string;
  userId: string;
  userHaveActiveTableReservation: boolean;
  tableReservationTime?: Date | null;
}
