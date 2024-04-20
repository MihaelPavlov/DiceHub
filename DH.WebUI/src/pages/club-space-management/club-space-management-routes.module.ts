import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ClubSpaceManagementComponent } from './page/club-space-management.component';

const routes: Routes = [
  {
    path: 'home',
    component: ClubSpaceManagementComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ClubSpaceManagementRoutingModule {}
