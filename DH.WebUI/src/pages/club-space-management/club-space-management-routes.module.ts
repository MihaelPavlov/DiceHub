import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ClubSpaceManagementComponent } from './page/club-space-management.component';
import { AddUpdateClubSpaceComponent } from '../../features/club-space-management/components/add-update-club-space/add-update-club-space.component';
import { ClubSpaceListComponent } from '../../features/club-space-management/components/club-space-list/club-space-list.component';
import { ClubSpaceDetailsComponent } from '../../features/club-space-management/components/club-space-details/club-space-details.component';
import { SpaceBookingComponent } from '../../features/club-space-management/components/space-booking/space-booking.component';
import { UserHasActiveTableGuard } from './guards/user-has-active-table.guard';
import { AuthGuard } from '../../shared/guards/auth.guard';

const routes: Routes = [
  {
    path: 'home',
    component: ClubSpaceManagementComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'create/:gameId',
    component: AddUpdateClubSpaceComponent,
    canActivate: [AuthGuard, UserHasActiveTableGuard],
  },
  {
    path: 'update/:tableId',
    component: AddUpdateClubSpaceComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'list',
    component: ClubSpaceListComponent,
    canActivate: [AuthGuard, UserHasActiveTableGuard],
  },
  {
    path: ':id/details',
    component: ClubSpaceDetailsComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'booking',
    component: SpaceBookingComponent,
    canActivate: [AuthGuard],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ClubSpaceManagementRoutingModule {}
