import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ReservationManagementNavigationComponent } from './page/reservation-management-navigation.component';
import { GameReservations } from '../../features/reservation-management/components/game-reservations/game-reservations.component';
import { SpaceTableReservations } from '../../features/reservation-management/components/space-table-reservations/space-table-reservations.component';

const routes: Routes = [
  {
    path: '',
    component: ReservationManagementNavigationComponent,
    children: [
      { path: 'games', component: GameReservations },
      { path: 'tables', component: SpaceTableReservations },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ReservationManagementRoutingModule {}
