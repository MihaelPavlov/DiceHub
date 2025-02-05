import { ReservationStatus } from '../../../shared/enums/reservation-status.enum';

export interface IGameReservationHistory {
  id: number;
  userId: string;
  username: string;
  numberOfGuests: number;
  reservationDate: Date;
  gameId: number;
  gameName: string;
  createdDate: Date;
  isActive: boolean;
  status: ReservationStatus;
  isReservationSuccessful: boolean;
  userHaveActiveTableReservation: boolean;
  tableReservationTime?: Date | null;
}
