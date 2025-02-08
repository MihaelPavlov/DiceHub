import {
  Component,
  ContentChild,
  EventEmitter,
  Input,
  Output,
  TemplateRef,
} from '@angular/core';
import { ReservationStatus } from '../../../../../shared/enums/reservation-status.enum';
import { ReservationType } from '../../../enums/reservation-type.enum';
import { DateHelper } from '../../../../../shared/helpers/date-helper';
import { Observable, BehaviorSubject } from 'rxjs';
import { IGameReservationHistory } from '../../../../../entities/games/models/game-reservation-history.model';
import { ITableReservationHistory } from '../../../../../entities/space-management/models/table-reservation-history.model';

@Component({
  selector: 'app-reservation-history-actions',
  templateUrl: 'reservation-history-actions.component.html',
  styleUrl: 'reservation-history-actions.component.scss',
})
export class ReservationHistoryActionsComponent<
  T extends IGameReservationHistory | ITableReservationHistory
> {
  @Input({ required: true }) historyList!: Observable<T[] | null>;
  @Input({ required: true }) expandedReservationId!: BehaviorSubject<
    number | null
  >;
  @Input({ required: true }) selectedFilter: ReservationStatus | null = null;

  @ContentChild(TemplateRef) contentTemplate!: TemplateRef<any>;

  @Output() updateReservationHistoryById: EventEmitter<number> =
    new EventEmitter<number>();

  @Output() deleteReservationHistoryById: EventEmitter<number> =
    new EventEmitter<number>();

  @Output() approveReservationHistory: EventEmitter<T> = new EventEmitter<T>();

  @Output() declineReservationHistory: EventEmitter<T> = new EventEmitter<T>();

  public isInfoForExpiredRecordVisible: boolean = false;

  public readonly ReservationType = ReservationType;
  public readonly ReservationStatus = ReservationStatus;
  public readonly DATE_TIME_FORMAT: string = DateHelper.DATE_TIME_FORMAT;

  public toggleItem(reservationId: number): void {
    this.expandedReservationId.next(
      this.expandedReservationId.value === reservationId ? null : reservationId
    );

    if (this.expandedReservationId.value === null)
      this.isInfoForExpiredRecordVisible = false;
  }

  public isExpanded(reservationId: number): boolean {
    return this.expandedReservationId.value === reservationId;
  }

  public updateReservation(id: number, event?: MouseEvent): void {
    if (event) event.stopPropagation();

    this.updateReservationHistoryById.emit(id);
  }

  public deleteReservation(id: number, event?: MouseEvent): void {
    if (event) event.stopPropagation();

    this.deleteReservationHistoryById.emit(id);
  }

  public showInfoForExpiredRecord(event?: MouseEvent): void {
    if (event) event.stopPropagation();

    this.isInfoForExpiredRecordVisible = !this.isInfoForExpiredRecordVisible;
  }

  public approveReservation(historyRecord: T, event?: MouseEvent): void {
    if (event) event.stopPropagation();

    this.approveReservationHistory.emit(historyRecord);
  }

  public declineReservation(historyRecord: T, event?: MouseEvent): void {
    if (event) event.stopPropagation();

    this.declineReservationHistory.emit(historyRecord);
  }
}
