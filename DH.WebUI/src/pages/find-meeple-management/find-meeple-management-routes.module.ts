import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FindMeepleManagementComponent } from './page/find-meeple-management.component';
import { MeepleRoomDetailsComponent } from '../../features/find-meeple-management/components/meeple-room-details/meeple-room-details.component';
import { RoomChatComponent } from '../../features/find-meeple-management/components/room-chat/room-chat.component';
import { RoomMembersComponent } from '../../features/find-meeple-management/components/room-members/room-members.component';
import { AddUpdateMeepleRoomComponent } from '../../features/find-meeple-management/components/add-update-meeple-room/add-update-meeple-room.component';

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
    component: AddUpdateMeepleRoomComponent,
  },
  {
    path: ':id/update',
    component: AddUpdateMeepleRoomComponent,
  },
  {
    path: ':id/chat',
    component: RoomChatComponent,
  },
  {
    path: ':id/chat/members',
    component: RoomMembersComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class FindMeepleManagementRoutingModule {}
