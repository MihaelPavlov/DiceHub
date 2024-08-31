import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { EventsLibraryComponent } from './page/events-library.component';
import { EventsLibraryRoutingModule } from './events-library-routes.module';
import { HeaderModule } from '../../widgets/header/header.module';
import { ChipModule } from '../../widgets/chip/chip.module';
import { EventDetailsComponent } from '../../features/events-library/components/event-details/page/event-details.component';

@NgModule({
  declarations: [
    EventsLibraryComponent,
    EventDetailsComponent,
  ],
  exports: [EventsLibraryComponent],
  providers: [],
  imports: [SharedModule, HeaderModule, EventsLibraryRoutingModule, ChipModule],
})
export class EventsLibraryModule {}
