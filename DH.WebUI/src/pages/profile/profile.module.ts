import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { HeaderModule } from '../../widgets/header/header.module';
import { ProfileComponent } from './page/profile.component';
import { ProfileRoutingModule } from './profile-routes.module';
import { GlobalSettingsComponent } from '../../features/profile/components/global-settings/global-settings.component';
import { NgSelectModule } from '@ng-select/ng-select';
import { UserSettingsComponent } from '../../features/profile/components/user-settings/user-settings.component';
import { EmployeeListComponent } from '../../features/profile/components/employee-list/employee-list.component';
import { AddEmployeeComponent } from '../../features/profile/components/add-employee/add-employee.component';

@NgModule({
  declarations: [
    ProfileComponent,
    GlobalSettingsComponent,
    UserSettingsComponent,
    EmployeeListComponent,
    AddEmployeeComponent,
  ],
  exports: [],
  providers: [],
  imports: [SharedModule, HeaderModule, ProfileRoutingModule, NgSelectModule],
})
export class ProfileModule {}
