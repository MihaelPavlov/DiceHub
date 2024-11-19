import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ClubSpaceManagementComponent } from './page/club-space-management.component';
import { AddUpdateClubSpaceComponent } from '../../features/club-space-management/components/add-update-club-space/add-update-club-space.component';
import { ClubSpaceListComponent } from '../../features/club-space-management/components/club-space-list/club-space-list.component';
import { ClubSpaceDetailsComponent } from '../../features/club-space-management/components/club-space-details/club-space-details.component';
import { UserHasActiveTableGuard } from '../../shared/guards/user-has-active-table.guard';
import { SpaceBookingComponent } from '../../features/club-space-management/components/space-booking/space-booking.component';

const routes: Routes = [
  {
    path: 'home',
    component: ClubSpaceManagementComponent,
  },
  {
    path: 'create/:gameId',
    component: AddUpdateClubSpaceComponent,
  },
  {
    path: 'update/:tableId',
    component: AddUpdateClubSpaceComponent,
  },
  {
    path: 'list',
    component: ClubSpaceListComponent,
    canActivate: [UserHasActiveTableGuard],
  },
  {
    path: ':id/details',
    component: ClubSpaceDetailsComponent,
  },
  {
    path: 'booking',
    component: SpaceBookingComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ClubSpaceManagementRoutingModule {}
