import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { HeaderModule } from '../../widgets/header/header.module';
import { FindMeepleManagementRoutingModule } from './find-meeple-management-routes.module';
import { FindMeepleManagementComponent } from './page/find-meeple-management.component';
import { ChipModule } from '../../widgets/chip/chip.module';
import { MeepleRoomDetailsComponent } from '../../features/find-meeple-management/components/meeple-room-details/meeple-room-details.component';

@NgModule({
  declarations: [FindMeepleManagementComponent, MeepleRoomDetailsComponent],
  exports: [FindMeepleManagementComponent],
  providers: [],
  imports: [
    SharedModule,
    HeaderModule,
    FindMeepleManagementRoutingModule,
    ChipModule,
  ],
})
export class FindMeepleMamagementModule {}
