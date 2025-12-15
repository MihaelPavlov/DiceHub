import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ReservationManagementNavigationComponent } from './page/reservation-management-navigation.component';
import { SpaceTableActiveReservations } from '../../features/reservation-management/components/space-table-reservations/active-list/space-table-active-reservations.component';
import { SpaceTableReservationHistory } from '../../features/reservation-management/components/space-table-reservations/history/space-table-reservation-history.component';
import { GameReservations } from '../../features/reservation-management/components/game-reservations/active-list/game-reservations.component';
import { GameReservationHistory } from '../../features/reservation-management/components/game-reservations/history/game-reservation-history.component';
import { AuthGuard } from '../../shared/guards/auth.guard';

const routes: Routes = [
  {
    path: '',
    component: ReservationManagementNavigationComponent,
    canActivate: [AuthGuard],
    children: [
      { path: 'games', component: GameReservations, canActivate: [AuthGuard] },
      {
        path: 'games/history',
        component: GameReservationHistory,
        canActivate: [AuthGuard],
      },
      {
        path: 'tables',
        component: SpaceTableActiveReservations,
        canActivate: [AuthGuard],
      },
      {
        path: 'tables/history',
        component: SpaceTableReservationHistory,
        canActivate: [AuthGuard],
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ReservationManagementRoutingModule {}
