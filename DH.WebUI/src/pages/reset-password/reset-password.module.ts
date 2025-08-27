import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { ResetPasswordComponent } from './page/reset-password.component';
import { LanguageSwitchModule } from "../../shared/components/language-switch/language-switch.module";

@NgModule({
  declarations: [ResetPasswordComponent],
  exports: [ResetPasswordComponent],
  providers: [],
  imports: [SharedModule, LanguageSwitchModule],
})
export class ResetPasswordModule {}
