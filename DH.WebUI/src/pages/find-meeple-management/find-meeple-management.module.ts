import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { HeaderModule } from '../../widgets/header/header.module';
import { FindMeepleManagementRoutingModule } from './find-meeple-management-routes.module';
import { FindMeepleManagementComponent } from './page/find-meeple-management.component';
import { ChipModule } from '../../widgets/chip/chip.module';
import { MeepleRoomDetailsComponent } from '../../features/find-meeple-management/components/meeple-room-details/meeple-room-details.component';
import { FormsModule } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';
import { RoomChatComponent } from '../../features/find-meeple-management/components/room-chat/room-chat.component';
import { RoomConfirmDeleteDialog } from '../../features/find-meeple-management/dialogs/room-confirm-delete/room-confirm-delete.component';
import {
  MatDialogActions,
  MatDialogClose,
  MatDialogModule,
} from '@angular/material/dialog';
import { RoomConfirmLeaveDialog } from '../../features/find-meeple-management/dialogs/room-confirm-leave/room-confirm-leave.component';
import { MeepleRoomMenuComponent } from '../../features/find-meeple-management/components/meeple-room-menu/meeple-room-menu.component';
import { RoomMembersComponent } from '../../features/find-meeple-management/components/room-members/room-members.component';
import { AddUpdateMeepleRoomComponent } from '../../features/find-meeple-management/components/add-update-meeple-room/add-update-meeple-room.component';
import { RoomMemberConfirmDeleteDialog } from '../../features/find-meeple-management/dialogs/room-member-confirm-delete/room-member-confirm-delete.component';

@NgModule({
  declarations: [
    FindMeepleManagementComponent,
    MeepleRoomDetailsComponent,
    AddUpdateMeepleRoomComponent,
    RoomChatComponent,
    MeepleRoomMenuComponent,
    RoomMembersComponent,
    RoomConfirmDeleteDialog,
    RoomConfirmLeaveDialog,
    RoomMemberConfirmDeleteDialog,
  ],
  exports: [FindMeepleManagementComponent],
  providers: [],
  imports: [
    SharedModule,
    HeaderModule,
    FindMeepleManagementRoutingModule,
    ChipModule,
    FormsModule,
    NgSelectModule,
    MatDialogActions,
    MatDialogClose,
    MatDialogModule,
  ],
})
export class FindMeepleManagementModule {}
