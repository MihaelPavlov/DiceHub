import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ClubSpaceManagementComponent } from './page/club-space-management.component';
import { AddUpdateClubSpaceComponent } from '../../features/club-space-management/components/add-update-club-space/add-update-club-space.component';
import { ClubSpaceListComponent } from '../../features/club-space-management/components/club-space-list/club-space-list.component';

const routes: Routes = [
  {
    path: 'home',
    component: ClubSpaceManagementComponent,
  },
  {
    path: 'create',
    component: AddUpdateClubSpaceComponent,
  },
  {
    path: 'list',
    component: ClubSpaceListComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ClubSpaceManagementRoutingModule {}
