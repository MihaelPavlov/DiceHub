import { ReservationStatus } from "../enums/reservation-status.enum";

export interface ActiveBookedTableModel {
  id: number;
  username: string;
  numberOfGuests: number;
  reservationDate: Date;
  createdDate: Date;
  isActive: boolean;
  status: ReservationStatus;
}

export function getKeyFriendlyNames(): Record<string, string> {
  return {
    username: 'Username',
    numberOfGuests: 'Number of Guests',
    reservationDate: 'Reservation Date',
    createdDate: 'Created Date',
    isConfirmed: 'Is Confirmed',
  };
}
