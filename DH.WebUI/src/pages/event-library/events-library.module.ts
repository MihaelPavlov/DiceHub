import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { EventsLibraryComponent } from './page/events-library.component';
import { EventsLibraryRoutingModule } from './events-library-routes.module';
import { HeaderModule } from '../../widgets/header/header.module';

@NgModule({
  declarations: [EventsLibraryComponent],
  exports: [EventsLibraryComponent],
  providers: [],
  imports: [SharedModule,HeaderModule,EventsLibraryRoutingModule],
})
export class EventsLibraryModule {}
