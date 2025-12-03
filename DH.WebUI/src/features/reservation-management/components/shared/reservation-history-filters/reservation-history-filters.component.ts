import { Component, EventEmitter, Output } from '@angular/core';
import { ReservationStatus } from '../../../../../shared/enums/reservation-status.enum';

@Component({
    selector: 'app-reservation-history-filters',
    templateUrl: 'reservation-history-filters.component.html',
    styleUrl: 'reservation-history-filters.component.scss',
    standalone: false
})
export class ReservationHistoryFiltersComponent {
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
