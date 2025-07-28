import { RouterModule, Routes } from '@angular/router';
import { ProfileComponent } from './page/profile.component';
import { NgModule } from '@angular/core';
import { GlobalSettingsComponent } from '../../features/profile/components/global-settings/global-settings.component';
import { UserSettingsComponent } from '../../features/profile/components/user-settings/user-settings.component';
import { SettingsUserAccessGuard } from './guards/settings-user-access.guard';
import { SettingsOwnerAccessGuard } from './guards/settings-owner-access.guard';
import { EmployeeListComponent } from '../../features/profile/components/employee-list/employee-list.component';
import { AddUpdateEmployeeComponent } from '../../features/profile/components/add-employee/add-update-employee.component';
import { OwnerDetailsComponent } from '../../features/profile/components/owner-details/owner-details.component';
import { SettingsSuperAdminAccessGuard } from './guards/settings-super-admin-access.guard';
import { JobsComponent } from '../../features/profile/components/jobs/jobs.component';

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
    canActivate: [SettingsOwnerAccessGuard],
  },
  {
    path: 'add-employee',
    component: AddUpdateEmployeeComponent,
    canActivate: [SettingsOwnerAccessGuard],
  },
  {
    path: ':id/update-employee',
    component: AddUpdateEmployeeComponent,
    canActivate: [SettingsOwnerAccessGuard],
  },
  {
    path: 'owner-details',
    component: OwnerDetailsComponent,
    canActivate: [SettingsSuperAdminAccessGuard],
  },
  {
    path: 'jobs',
    component: JobsComponent,
    canActivate: [SettingsSuperAdminAccessGuard],
  },
  // {
  //   path: 'add-owner',
  //   component: AddOwnerComponent,
  //   canActivate: [SettingsSuperAdminAccessGuard],
  // },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ProfileRoutingModule {}
