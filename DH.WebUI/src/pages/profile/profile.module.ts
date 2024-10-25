import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { HeaderModule } from '../../widgets/header/header.module';
import { ProfileComponent } from './page/profile.component';
import { ProfileRoutingModule } from './profile-routes.module';

@NgModule({
  declarations: [ProfileComponent],
  exports: [],
  providers: [],
  imports: [SharedModule, HeaderModule, ProfileRoutingModule],
})
export class ProfileModule {}
