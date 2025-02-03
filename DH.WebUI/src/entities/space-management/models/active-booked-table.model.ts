import { ReservationStatus } from '../../../shared/enums/reservation-status.enum';

export interface ActiveBookedTableModel {
  id: number;
  username: string;
  numberOfGuests: number;
  reservationDate: Date;
  createdDate: Date;
  isActive: boolean;
  status: ReservationStatus;
  publicNote: string;
}

export function getKeyFriendlyNames(): Record<string, string> {
  return {
    numberOfGuests: 'Number of Guests',
    reservationDate: 'Reservation Date',
    isConfirmed: 'Is Confirmed',
  };
}
