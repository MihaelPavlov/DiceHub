import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { HeaderModule } from '../../widgets/header/header.module';
import { ProfileComponent } from './page/profile.component';
import { ProfileRoutingModule } from './profile-routes.module';
import { GlobalSettingsComponent } from '../../features/profile/components/global-settings/global-settings.component';
import { NgSelectModule } from '@ng-select/ng-select';
import { UserSettingsComponent } from '../../features/profile/components/user-settings/user-settings.component';
import { EmployeeListComponent } from '../../features/profile/components/employee-list/employee-list.component';
import { AddUpdateEmployeeComponent } from '../../features/profile/components/add-employee/add-update-employee.component';
import { EmployeeConfirmDeleteDialog } from '../../features/profile/dialogs/employee-confirm-delete/employee-confirm-delete.component';
import { MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import { OwnerDetailsComponent } from '../../features/profile/components/owner-details/owner-details.component';
import { JobsComponent } from '../../features/profile/components/jobs/jobs.component';

@NgModule({
  declarations: [
    ProfileComponent,
    GlobalSettingsComponent,
    UserSettingsComponent,
    EmployeeListComponent,
    AddUpdateEmployeeComponent,
    EmployeeConfirmDeleteDialog,
    OwnerDetailsComponent,
    JobsComponent
  ],
  exports: [],
  providers: [],
  imports: [
    SharedModule,
    HeaderModule,
    ProfileRoutingModule,
    MatDialogActions,
    MatDialogClose,
    NgSelectModule,
  ],
})
export class ProfileModule {}
