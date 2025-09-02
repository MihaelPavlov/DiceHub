import { TranslateService } from '@ngx-translate/core';
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

export function getKeyFriendlyNames(
  ts: TranslateService
): Record<string, string> {
  return {
    numberOfGuests: ts.instant(
      'space_management.reservation_keys.number_of_guests'
    ),
    reservationDate: ts.instant(
      'space_management.reservation_keys.reservation_date'
    ),
    isConfirmed: ts.instant('space_management.reservation_keys.is_confirmed'),
  };
}
