import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { HeaderModule } from '../../widgets/header/header.module';
import { FindMeepleManagementRoutingModule } from './find-meeple-management-routes.module';
import { FindMeepleManagementComponent } from './page/find-meeple-management.component';
import { ChipModule } from '../../widgets/chip/chip.module';
import { MeepleRoomDetailsComponent } from '../../features/find-meeple-management/components/meeple-room-details/meeple-room-details.component';
import { CreateMeepleRoomComponent } from '../../features/find-meeple-management/components/create-meeple-room/create-meeple-room.component';
import { FormsModule } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';
import { RoomChatComponent } from '../../features/find-meeple-management/components/room-chat/room-chat.component';

@NgModule({
  declarations: [
    FindMeepleManagementComponent,
    MeepleRoomDetailsComponent,
    CreateMeepleRoomComponent,
    RoomChatComponent,
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
  ],
})
export class FindMeepleMamagementModule {}
