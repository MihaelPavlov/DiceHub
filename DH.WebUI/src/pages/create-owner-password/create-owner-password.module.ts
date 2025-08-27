import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { CreateOwnerPasswordComponent } from './page/create-owner-password.component';
import { LanguageSwitchModule } from '../../shared/components/language-switch/language-switch.module';

@NgModule({
  declarations: [CreateOwnerPasswordComponent],
  exports: [CreateOwnerPasswordComponent],
  providers: [],
  imports: [SharedModule, LanguageSwitchModule],
})
export class CreateOwnerPasswordModule {}
