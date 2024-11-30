import { Component, Injector, OnDestroy } from '@angular/core';
import { ReservationManagementNavigationComponent } from '../../../../pages/reservation-management/page/reservation-management-navigation.component';

@Component({
  selector: 'app-space-table-reservations',
  templateUrl: 'space-table-reservations.component.html',
  styleUrl: 'space-table-reservations.component.scss',
})
export class SpaceTableReservations implements OnDestroy {
  private reservationNavigationRef!: ReservationManagementNavigationComponent | null;

  constructor(private readonly injector: Injector) {
    this.reservationNavigationRef = this.injector.get(
      ReservationManagementNavigationComponent,
      null
    );
  }

  public ngOnDestroy(): void {
    this.reservationNavigationRef?.removeActiveChildComponent();
  }
}
