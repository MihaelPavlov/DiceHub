import { ReservationStatus } from '../../../shared/enums/reservation-status.enum';

export interface IGameReservationStatus {
  reservationId: number;
  gameId: number;
  reservationDate: Date;
  reservedDurationMinutes: number;
  isActive: boolean;
  status: ReservationStatus;
  publicNote: string;
}
