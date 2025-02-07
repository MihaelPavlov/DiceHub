import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ReservationStatus } from '../../../../../shared/enums/reservation-status.enum';
import { ReservationType } from '../../../enums/reservation-type.enum';

@Component({
  selector: 'app-reservation-history-filters',
  templateUrl: 'reservation-history-filters.component.html',
  styleUrl: 'reservation-history-filters.component.scss',
})
export class ReservationHistoryFiltersComponent {
  @Input({ required: true }) reservationType!: ReservationType;

  @Output() expandedReservationIdReset: EventEmitter<void> =
    new EventEmitter<void>();
  @Output() updateHistory: EventEmitter<ReservationStatus | null> =
    new EventEmitter<ReservationStatus | null>();

  public selectedFilter: ReservationStatus | null = null;

  public readonly ReservationStatus = ReservationStatus;

  public applyFilter(filter: ReservationStatus | null = null): void {
    this.selectedFilter = filter;
    this.expandedReservationIdReset.emit();
    this.updateHistory.emit(filter);
  }
}
