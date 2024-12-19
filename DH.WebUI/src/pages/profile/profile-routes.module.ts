import { RouterModule, Routes } from '@angular/router';
import { ProfileComponent } from './page/profile.component';
import { NgModule } from '@angular/core';
import { GlobalSettingsComponent } from '../../features/profile/components/global-settings/global-settings.component';
import { UserSettingsComponent } from '../../features/profile/components/user-settings/user-settings.component';
import { SettingsUserAccessGuard } from './guards/settings-user-access.guard';
import { SettingsOwnerAccessGuard } from './guards/settings-owner-access.guard';
import { EmployeeListComponent } from '../../features/profile/components/employee-list/employee-list.component';
import { AddEmployeeComponent } from '../../features/profile/components/add-employee/add-employee.component';

const routes: Routes = [
  {
    path: '',
    component: ProfileComponent,
  },
  {
    path: 'settings',
    component: GlobalSettingsComponent,
    canActivate: [SettingsOwnerAccessGuard],
  },
  {
    path: 'user-settings',
    component: UserSettingsComponent,
    canActivate: [SettingsUserAccessGuard],
  },
  {
    path: 'employees',
    component: EmployeeListComponent,
    // canActivate: [SettingsUserAccessGuard],
  },
  {
    path: 'add-employee',
    component: AddEmployeeComponent,
    // canActivate: [SettingsUserAccessGuard],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ProfileRoutingModule {}
