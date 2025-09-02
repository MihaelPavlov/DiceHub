import { ReservationStatus } from '../../../../shared/enums/reservation-status.enum';
import { ReservationType } from '../../enums/reservation-type.enum';

export interface ReservationConfirmation {
  reservationId: number;
  type: ReservationType;
  status: ReservationStatus;
  reservationDate: Date;
  numberOfGuests: number;
  gameName?: string | null;
  phoneNumber: string;
  userLanguage: string;
  tableReservationDate?: Date | null;
}
