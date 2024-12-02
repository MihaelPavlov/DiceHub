import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { HeaderModule } from '../../widgets/header/header.module';
import { ClubSpaceManagementRoutingModule } from './club-space-management-routes.module';
import { ClubSpaceManagementComponent } from './page/club-space-management.component';
import { AddUpdateClubSpaceComponent } from '../../features/club-space-management/components/add-update-club-space/add-update-club-space.component';
import { ClubSpaceListComponent } from '../../features/club-space-management/components/club-space-list/club-space-list.component';
import { ChipModule } from '../../widgets/chip/chip.module';
import { SinglePlayerConfirmDialog } from '../../features/club-space-management/dialogs/single-player-confirm-dialog/single-player-confirm-dialog.component';
import {
  MatDialogActions,
  MatDialogClose,
  MatDialogContent,
  MatDialogModule,
  MatDialogTitle,
} from '@angular/material/dialog';
import { ClubSpaceDetailsComponent } from '../../features/club-space-management/components/club-space-details/club-space-details.component';
import { JoinTableConfirmDialog } from '../../features/club-space-management/dialogs/join-table-confirm-dialog/join-table-confirm-dialog.component';
import { SpaceBookingComponent } from '../../features/club-space-management/components/space-booking/space-booking.component';
import { DiceRollerComponent } from '../../features/club-space-management/components/space-booking/components/dice-scroller/dice-roller.component';
import { ReservationQrCodeDialogModule } from '../../shared/dialogs/reservation-qr-code/reservation-qr-code.module';

@NgModule({
  declarations: [
    ClubSpaceManagementComponent,
    AddUpdateClubSpaceComponent,
    ClubSpaceListComponent,
    ClubSpaceDetailsComponent,
    SinglePlayerConfirmDialog,
    JoinTableConfirmDialog,
    DiceRollerComponent,
    SpaceBookingComponent
  ],
  exports: [ClubSpaceManagementComponent],
  providers: [],
  imports: [
    SharedModule,
    HeaderModule,
    ClubSpaceManagementRoutingModule,
    ChipModule,
    MatDialogTitle,
    MatDialogContent,
    MatDialogActions,
    MatDialogClose,
    MatDialogModule,
    ReservationQrCodeDialogModule
  ],
})
export class ClubSpaceManagementModule {}
