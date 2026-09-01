import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { RegisterComponent } from './page/register.component';
import { LanguageSwitchModule } from '../../shared/components/language-switch/language-switch.module';
import { RegisterChoiceComponent } from '../register-choice/page/register-choice.component';
import { VenueApplicationComponent } from '../venue-application/page/venue-application.component';
import { TenantSetupComponent } from '../tenant-setup/page/tenant-setup.component';
import { NgSelectModule } from '@ng-select/ng-select';

@NgModule({
  declarations: [
    RegisterComponent,
    RegisterChoiceComponent,
    VenueApplicationComponent,
    TenantSetupComponent,
  ],
  exports: [
    RegisterComponent,
    RegisterChoiceComponent,
    VenueApplicationComponent,
    TenantSetupComponent,
  ],
  providers: [],
  imports: [SharedModule, LanguageSwitchModule, NgSelectModule],
})
export class RegisterModule {}
