import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';
import { EventDetailsComponent } from '../../features/events-library/components/event-details/page/event-details.component';
import { AdminEventManagementComponent } from './admin-page/admin-event-management.component';
import { AddUpdateEventComponent } from '../../features/events-library/components/add-update-event/page/add-update-event.component';
import { AdminEventDetailsComponent } from '../../features/events-library/components/admin-event-details/page/admin-event-details.component';
import { EventAdminAccessGuard } from '../../shared/guards/event-admin-access.guard';
import { EventUserAccessGuard } from '../../shared/guards/event-user.guard';
import { EventsLibraryComponent } from './page/events-library.component';
import { ROUTE } from '../../shared/configs/route.config';

const routes: Routes = [
  {
    path: ROUTE.EVENTS.HOME,
    component: EventsLibraryComponent,
    canActivate: [EventUserAccessGuard],
  },
  {
    path: `:id/${ROUTE.EVENTS.DETAILS}`,
    component: EventDetailsComponent,
    canActivate: [EventUserAccessGuard],
  },
  {
    path: ROUTE.EVENTS.ADMIN.CORE,
    component: AdminEventManagementComponent,
    canActivate: [EventAdminAccessGuard],
  },
  {
    path: `${ROUTE.EVENTS.ADMIN.CORE}/${ROUTE.EVENTS.ADMIN.ADD}`,
    component: AddUpdateEventComponent,
    canActivate: [EventAdminAccessGuard],
  },
  {
    path: `:id/${ROUTE.EVENTS.ADMIN.CORE}/${ROUTE.EVENTS.ADMIN.UPDATE}`,
    component: AddUpdateEventComponent,
    canActivate: [EventAdminAccessGuard],
  },
  {
    path: `:id/${ROUTE.EVENTS.ADMIN.CORE}/${ROUTE.EVENTS.ADMIN.DETAILS}`,
    component: AdminEventDetailsComponent,
    canActivate: [EventAdminAccessGuard],
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class EventsLibraryRoutingModule {}
