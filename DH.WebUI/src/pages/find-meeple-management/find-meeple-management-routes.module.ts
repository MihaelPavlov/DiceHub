import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FindMeepleManagementComponent } from './page/find-meeple-management.component';
import { MeepleRoomDetailsComponent } from '../../features/find-meeple-management/components/meeple-room-details/meeple-room-details.component';
import { CreateMeepleRoomComponent } from '../../features/find-meeple-management/components/create-meeple-room/create-meeple-room.component';

const routes: Routes = [
  {
    path: 'find',
    component: FindMeepleManagementComponent,
  },
  {
    path: ':id/details',
    component: MeepleRoomDetailsComponent,
  },
  {
    path: 'create',
    component: CreateMeepleRoomComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class FindMeepleManagementRoutingModule {}
