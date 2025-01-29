import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ReservationStatus } from '../../../../shared/enums/reservation-status.enum';

interface AvailabilityReservationInfoDialogData {
  publicNote: string;
  status: ReservationStatus;
}

@Component({
  selector: 'app-availability-reservation-info-dialog',
  templateUrl: 'availability-reservation-info-dialog.component.html',
  styleUrl: 'availability-reservation-info-dialog.component.scss',
})
export class AvailabilityReservationInfoDialog {
  public readonly ReservationStatus = ReservationStatus;
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: AvailabilityReservationInfoDialogData
  ) {}
}
