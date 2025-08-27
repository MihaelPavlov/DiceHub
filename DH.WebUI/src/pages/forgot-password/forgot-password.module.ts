import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { ForgotPasswordComponent } from './page/forgot-password.component';
import { LanguageSwitchModule } from '../../shared/components/language-switch/language-switch.module';

@NgModule({
  declarations: [ForgotPasswordComponent],
  exports: [ForgotPasswordComponent],
  providers: [],
  imports: [SharedModule, LanguageSwitchModule],
})
export class ForgotPasswordModule {}
