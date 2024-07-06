import { RouterModule, Routes } from '@angular/router';
import { AdminSpaceComponent } from './page/admin-space.component';
import { NgModule } from '@angular/core';
import { AddUpdateUserComponent } from '../../features/admin-space/components/add-update-user/add-update-user.component';

const routes: Routes = [
  {
    path: '',
    component: AdminSpaceComponent,
  },
  {
    path: 'add/user',
    component: AddUpdateUserComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminSpaceRoutingModule {}
