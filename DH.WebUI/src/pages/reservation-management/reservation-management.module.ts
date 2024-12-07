import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { HeaderModule } from '../../widgets/header/header.module';
import { ReservationManagementRoutingModule } from './reservation-management-routes.module';
import { ReservationManagementNavigationComponent } from './page/reservation-management-navigation.component';
import { NavBarModule } from '../../widgets/nav-bar/nav-bar.module';
import { GameReservations } from '../../features/reservation-management/components/game-reservations/game-reservations.component';
import { NgSelectModule } from '@ng-select/ng-select';
import { ReservationConfirmationDialog } from '../../features/reservation-management/dialogs/reservation-status-confirmation/reservation-confirmation.dialog';
import { MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import { SpaceTableActiveReservations } from '../../features/reservation-management/components/space-table-reservations/active-list/space-table-active-reservations.component';
import { SpaceTableReservationHistory } from '../../features/reservation-management/components/space-table-reservations/history/space-table-reservation-history.component';

@NgModule({
  declarations: [
    ReservationManagementNavigationComponent,
    GameReservations,
    SpaceTableActiveReservations,
    ReservationConfirmationDialog,
    SpaceTableReservationHistory
  ],
  exports: [],
  providers: [],
  imports: [
    SharedModule,
    HeaderModule,
    ReservationManagementRoutingModule,
    NavBarModule,
    NgSelectModule,
    MatDialogActions,
    MatDialogClose
  ],
})
export class ReservationManagementModule {}
