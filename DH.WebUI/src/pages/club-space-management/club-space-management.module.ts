import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { HeaderModule } from '../../widgets/header/header.module';
import { ClubSpaceManagementRoutingModule } from './club-space-management-routes.module';
import { ClubSpaceManagementComponent } from './page/club-space-management.component';

@NgModule({
  declarations: [ClubSpaceManagementComponent],
  exports: [ClubSpaceManagementComponent],
  providers: [],
  imports: [SharedModule, HeaderModule, ClubSpaceManagementRoutingModule],
})
export class ClubSpaceManagementModule {}
