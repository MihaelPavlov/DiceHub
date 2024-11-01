import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { EventsLibraryComponent } from './page/events-library.component';
import { EventsLibraryRoutingModule } from './events-library-routes.module';
import { HeaderModule } from '../../widgets/header/header.module';
import { ChipModule } from '../../widgets/chip/chip.module';
import { EventDetailsComponent } from '../../features/events-library/components/event-details/page/event-details.component';
import { AdminEventDetailsComponent } from '../../features/events-library/components/admin-event-details/page/admin-event-details.component';
import { AdminEventManagementComponent } from './admin-page/admin-event-management.component';
import { AddUpdateEventComponent } from '../../features/events-library/components/add-update-event/page/add-update-event.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';

@NgModule({
  declarations: [
    EventsLibraryComponent,
    EventDetailsComponent,
    AdminEventManagementComponent,
    AddUpdateEventComponent,
    AdminEventDetailsComponent,
  ],
  exports: [],
  providers: [],
  imports: [
    SharedModule,
    HeaderModule,
    EventsLibraryRoutingModule,
    ChipModule,
    FormsModule,
    ReactiveFormsModule,
    NgSelectModule,
  ],
})
export class EventsLibraryModule {}
