import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { HeaderModule } from '../../widgets/header/header.module';
import { ReservationManagementRoutingModule } from './reservation-management-routes.module';
import { ReservationManagementNavigationComponent } from './page/reservation-management-navigation.component';
import { NavBarModule } from '../../widgets/nav-bar/nav-bar.module';
import { NgSelectModule } from '@ng-select/ng-select';
import { ReservationConfirmationDialog } from '../../features/reservation-management/dialogs/reservation-status-confirmation/reservation-confirmation.dialog';
import { MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import { SpaceTableActiveReservations } from '../../features/reservation-management/components/space-table-reservations/active-list/space-table-active-reservations.component';
import { SpaceTableReservationHistory } from '../../features/reservation-management/components/space-table-reservations/history/space-table-reservation-history.component';
import { ReservationDetailsDialog } from '../../features/reservation-management/dialogs/reservation-details/reservation-details.dialog';
import { GameReservations } from '../../features/reservation-management/components/game-reservations/active-list/game-reservations.component';
import { GameReservationHistory } from '../../features/reservation-management/components/game-reservations/history/game-reservation-history.component';
import { ReservationHistoryFiltersComponent } from '../../features/reservation-management/components/shared/reservation-history-filters/reservation-history-filters.component';
import { ReservationHistoryActionsComponent } from '../../features/reservation-management/components/shared/reservation-history-actions/reservation-history-actions.component';

@NgModule({
  declarations: [
    ReservationManagementNavigationComponent,
    GameReservations,
    SpaceTableActiveReservations,
    ReservationConfirmationDialog,
    ReservationDetailsDialog,
    SpaceTableReservationHistory,
    GameReservationHistory,
    ReservationHistoryFiltersComponent,
    ReservationHistoryActionsComponent,
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
    MatDialogClose,
  ],
})
export class ReservationManagementModule {}
