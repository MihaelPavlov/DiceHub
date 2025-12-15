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
import { ClubInfo } from '../../features/profile/components/club-info/club-info.component';
import { AuthGuard } from '../../shared/guards/auth.guard';

const routes: Routes = [
  {
    path: '',
    component: ProfileComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'settings',
    component: GlobalSettingsComponent,
    canActivate: [AuthGuard, SettingsOwnerAccessGuard],
  },
  {
    path: 'user-settings',
    component: UserSettingsComponent,
    canActivate: [AuthGuard, SettingsUserAccessGuard],
  },
  {
    path: 'employees',
    component: EmployeeListComponent,
    canActivate: [AuthGuard, SettingsOwnerAccessGuard],
  },
  {
    path: 'add-employee',
    component: AddUpdateEmployeeComponent,
    canActivate: [AuthGuard, SettingsOwnerAccessGuard],
  },
  {
    path: ':id/update-employee',
    component: AddUpdateEmployeeComponent,
    canActivate: [AuthGuard, SettingsOwnerAccessGuard],
  },
  {
    path: 'owner-details',
    component: OwnerDetailsComponent,
    canActivate: [AuthGuard, SettingsSuperAdminAccessGuard],
  },
  {
    path: 'jobs',
    component: JobsComponent,
    canActivate: [AuthGuard, SettingsSuperAdminAccessGuard],
  },
  {
    path: 'club-info',
    component: ClubInfo,
    canActivate: [AuthGuard],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ProfileRoutingModule {}
