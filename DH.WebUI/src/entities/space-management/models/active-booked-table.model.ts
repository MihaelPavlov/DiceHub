export interface ActiveBookedTableModel {
  username: string;
  numberOfGuests: number;
  reservationDate: Date;
  createdDate: Date;
  isActive: boolean;
  isConfirmed: boolean;
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
