import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminEventManagementComponent } from './page/admin-event-management.component';
import { AddUpdateEventComponent } from '../../features/events-library/components/add-update-event/page/add-update-event.component';

const routes: Routes = [
  {
    path: '',
    component: AdminEventManagementComponent,
  },
  {
    path: 'add',
    component: AddUpdateEventComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminEventManagementRoutingModule {}
