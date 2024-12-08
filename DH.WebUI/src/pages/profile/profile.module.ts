import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { HeaderModule } from '../../widgets/header/header.module';
import { ProfileComponent } from './page/profile.component';
import { ProfileRoutingModule } from './profile-routes.module';
import { GlobalSettingsComponent } from '../../features/profile/components/global-settings/global-settings.component';
import { NgSelectModule } from '@ng-select/ng-select';

@NgModule({
  declarations: [ProfileComponent, GlobalSettingsComponent],
  exports: [],
  providers: [],
  imports: [SharedModule, HeaderModule, ProfileRoutingModule, NgSelectModule],
})
export class ProfileModule {}
