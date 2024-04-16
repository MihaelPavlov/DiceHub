import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FindMeepleManagementComponent } from './page/find-meeple-management.component';
import { MeepleRoomDetailsComponent } from '../../features/find-meeple-management/components/meeple-room-details/meeple-room-details.component';

const routes: Routes = [
  {
    path: 'find',
    component: FindMeepleManagementComponent,
  },
  {
    path: '1/details',
    component: MeepleRoomDetailsComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class FindMeepleManagementRoutingModule {}
