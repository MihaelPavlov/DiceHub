import { Component, Injector, OnDestroy } from '@angular/core';
import { ReservationManagementNavigationComponent } from '../../../../pages/reservation-management/page/reservation-management-navigation.component';

@Component({
  selector: 'app-game-reservations',
  templateUrl: 'game-reservations.component.html',
  styleUrl: 'game-reservations.component.scss',
})
export class GameReservations implements OnDestroy {
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
