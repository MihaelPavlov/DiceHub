import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { HeaderModule } from '../../widgets/header/header.module';
import { AdminEventManagementRoutingModule } from './admin-event-management-routes.module';
import { AdminEventManagementComponent } from './page/admin-event-management.component';
import { NgModule } from '@angular/core';
import { ChipModule } from '../../widgets/chip/chip.module';
import { AddUpdateEventComponent } from '../../features/events-library/components/add-update-event/page/add-update-event.component';

@NgModule({
  declarations: [AdminEventManagementComponent,AddUpdateEventComponent],
  exports: [],
  providers: [],
  imports: [
    SharedModule,
    HeaderModule,
    FormsModule,
    ReactiveFormsModule,
    ChipModule,
    AdminEventManagementRoutingModule,
  ],
})
export class AdminEventManagementModule {}
