import { NgModule } from '@angular/core';
import { AdminSpaceComponent } from './page/admin-space.component';
import { SharedModule } from '../../shared/shared.module';
import { AdminSpaceRoutingModule } from './admin-space-routing.module';
import { AddUpdateUserComponent } from '../../features/admin-space/components/add-update-user/add-update-user.component';

@NgModule({
  declarations: [AdminSpaceComponent,AddUpdateUserComponent],
  exports: [AdminSpaceComponent],
  providers: [],
  imports: [SharedModule, AdminSpaceRoutingModule],
})
export class AdminSpaceModule {}
