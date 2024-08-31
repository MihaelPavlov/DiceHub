import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminEventManagementComponent } from './page/admin-event-management.component';
import { AdminEventDetailsComponent } from '../../features/admin-event-management/components/event-details/page/admin-event-details.component';
import { AddUpdateEventComponent } from '../../features/admin-event-management/components/add-update-event/page/add-update-event.component';

const routes: Routes = [
  {
    path: '',
    component: AdminEventManagementComponent,
  },
  {
    path: 'add',
    component: AddUpdateEventComponent,
  },
  {
    path: ':id/update',
    component: AddUpdateEventComponent,
  },
  {
    path: ':id/details',
    component: AdminEventDetailsComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminEventManagementRoutingModule {}
