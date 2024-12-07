import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ReservationManagementNavigationComponent } from './page/reservation-management-navigation.component';
import { GameReservations } from '../../features/reservation-management/components/game-reservations/game-reservations.component';
import { SpaceTableActiveReservations } from '../../features/reservation-management/components/space-table-reservations/active-list/space-table-active-reservations.component';
import { SpaceTableConfirmedReservations } from '../../features/reservation-management/components/space-table-reservations/confirmed-list/space-table-confirmed-reservations.component';

const routes: Routes = [
  {
    path: '',
    component: ReservationManagementNavigationComponent,
    children: [
      { path: 'games', component: GameReservations },
      { path: 'tables', component: SpaceTableActiveReservations },
      { path: 'confirmed-tables', component: SpaceTableConfirmedReservations },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ReservationManagementRoutingModule {}
