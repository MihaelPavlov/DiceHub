import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { HeaderModule } from '../../widgets/header/header.module';
import { ReservationManagementRoutingModule } from './reservation-management-routes.module';
import { ReservationManagementNavigationComponent } from './page/reservation-management-navigation.component';
import { NavBarModule } from '../../widgets/nav-bar/nav-bar.module';
import { GameReservations } from '../../features/reservation-management/components/game-reservations/game-reservations.component';
import { SpaceTableReservations } from '../../features/reservation-management/components/space-table-reservations/space-table-reservations.component';

@NgModule({
  declarations: [
    ReservationManagementNavigationComponent,
    GameReservations,
    SpaceTableReservations,
  ],
  exports: [],
  providers: [],
  imports: [
    SharedModule,
    HeaderModule,
    ReservationManagementRoutingModule,
    NavBarModule,
  ],
})
export class ReservationManagementModule {}
