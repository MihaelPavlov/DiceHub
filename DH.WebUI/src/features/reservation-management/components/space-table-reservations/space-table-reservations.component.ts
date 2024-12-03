import { Component, Injector, OnDestroy } from '@angular/core';
import { ReservationManagementNavigationComponent } from '../../../../pages/reservation-management/page/reservation-management-navigation.component';
import { Observable } from 'rxjs';
import { IReservedTable } from '../../../../entities/space-management/models/reserved-table.model';
import { SpaceManagementService } from '../../../../entities/space-management/api/space-management.service';
import { ToastService } from '../../../../shared/services/toast.service';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../../shared/models/toast.model';
import { ReservationStatus } from '../../../../entities/space-management/enums/reservation-status.enum';

@Component({
  selector: 'app-space-table-reservations',
  templateUrl: 'space-table-reservations.component.html',
  styleUrl: 'space-table-reservations.component.scss',
})
export class SpaceTableReservations implements OnDestroy {
  private reservationNavigationRef!: ReservationManagementNavigationComponent | null;

  public reservedGames$!: Observable<IReservedTable[]>;
  public showFilter: boolean = false;
  public expandedReservationId: number | null = null;
  public leftArrowKey: string = 'arrow_circle_left';
  public rightArrowKey: string = 'arrow_circle_right';
  public ReservationStatus = ReservationStatus;
  constructor(
    private readonly injector: Injector,
    private readonly toastService: ToastService,
    private readonly spaceManagementService: SpaceManagementService
  ) {
    this.reservationNavigationRef = this.injector.get(
      ReservationManagementNavigationComponent,
      null
    );
  }

  public ngOnInit(): void {
    this.reservedGames$ = this.spaceManagementService.getReservedTableList();
  }

  public ngOnDestroy(): void {
    if (this.reservationNavigationRef)
      this.reservationNavigationRef.removeActiveChildComponent();
  }

  public toggleItem(
    reservationId: number,
    reservationStatus: ReservationStatus
  ): void {
    if (reservationStatus !== ReservationStatus.None) {
      return;
    }
    this.expandedReservationId =
      this.expandedReservationId === reservationId ? null : reservationId;
  }

  public isExpanded(reservationId: number): boolean {
    return this.expandedReservationId === reservationId;
  }

  public approveReservation(): void {
    if (this.expandedReservationId) {
      this.spaceManagementService
        .approveReservation(this.expandedReservationId)
        .subscribe({
          next: () => {
            this.toastService.success({
              message: 'Reservation is approved',
              type: ToastType.Success,
            });

            this.reservedGames$ =
              this.spaceManagementService.getReservedTableList();
            this.expandedReservationId = null;
          },
          error: () => {
            this.toastService.error({
              message: AppToastMessage.SomethingWrong,
              type: ToastType.Error,
            });
          },
        });
    }
  }

  public declineReservation(): void {
    if (this.expandedReservationId) {
      this.spaceManagementService
        .declinedReservation(this.expandedReservationId)
        .subscribe({
          next: () => {
            this.toastService.success({
              message: 'Reservation is declined',
              type: ToastType.Success,
            });

            this.reservedGames$ =
              this.spaceManagementService.getReservedTableList();

            this.expandedReservationId = null;
          },
          error: () => {
            this.toastService.error({
              message: AppToastMessage.SomethingWrong,
              type: ToastType.Error,
            });
          },
        });
    }
  }
}
