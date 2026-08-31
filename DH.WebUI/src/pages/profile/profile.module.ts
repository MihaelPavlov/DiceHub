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
import { ClubInfo } from '../../features/profile/components/club-info/club-info.component';
import { TenantApplicationsComponent } from '../../features/profile/components/tenant-applications/tenant-applications.component';
import { TenantApplicationDetailsComponent } from '../../features/profile/components/tenant-application-details/tenant-application-details.component';
import { SuperadminTenantsComponent } from '../../features/profile/components/superadmin-tenants/superadmin-tenants.component';
import { SuperadminTenantDetailsComponent } from '../../features/profile/components/superadmin-tenant-details/superadmin-tenant-details.component';
import { ChangePasswordComponent } from '../../features/profile/components/change-password/change-password.component';
import { ProfileSettingsNavComponent } from '../../features/profile/components/profile-settings-nav/profile-settings-nav.component';

@NgModule({
  declarations: [
    ProfileComponent,
    ProfileSettingsNavComponent,
    GlobalSettingsComponent,
    UserSettingsComponent,
    ChangePasswordComponent,
    EmployeeListComponent,
    AddUpdateEmployeeComponent,
    EmployeeConfirmDeleteDialog,
    OwnerDetailsComponent,
    JobsComponent,
    ClubInfo,
    TenantApplicationsComponent,
    TenantApplicationDetailsComponent,
    SuperadminTenantsComponent,
    SuperadminTenantDetailsComponent
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
