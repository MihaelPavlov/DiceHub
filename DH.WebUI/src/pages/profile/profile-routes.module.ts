import { RouterModule, Routes } from '@angular/router';
import { ProfileComponent } from './page/profile.component';
import { NgModule } from '@angular/core';
import { GlobalSettingsComponent } from '../../features/profile/components/global-settings/global-settings.component';
import { UserSettingsComponent } from '../../features/profile/components/user-settings/user-settings.component';
import { SettingsUserAccessGuard } from './guards/settings-user-access.guard';
import { SettingsOwnerAccessGuard } from './guards/settings-owner-access.guard';

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
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ProfileRoutingModule {}
