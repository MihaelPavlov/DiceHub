import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { HeaderModule } from '../../widgets/header/header.module';
import { AdminEventManagementRoutingModule } from './admin-event-management-routes.module';
import { AdminEventManagementComponent } from './page/admin-event-management.component';
import { NgModule } from '@angular/core';
import { ChipModule } from '../../widgets/chip/chip.module';
import { NgSelectModule } from '@ng-select/ng-select';
import { CalculateRemainingDaysPipe } from '../../features/admin-event-management/pipes/calculate-remaining-days.pipe';
import { AddUpdateEventComponent } from '../../features/admin-event-management/components/add-update-event/page/add-update-event.component';
import { AdminEventDetailsComponent } from '../../features/admin-event-management/components/event-details/page/admin-event-details.component';

@NgModule({
  declarations: [
    AdminEventManagementComponent,
    AddUpdateEventComponent,
    CalculateRemainingDaysPipe,
    AdminEventDetailsComponent
  ],
  exports: [],
  providers: [],
  imports: [
    SharedModule,
    HeaderModule,
    FormsModule,
    ReactiveFormsModule,
    ChipModule,
    NgSelectModule,
    AdminEventManagementRoutingModule,
  ],
})
export class AdminEventManagementModule {}
