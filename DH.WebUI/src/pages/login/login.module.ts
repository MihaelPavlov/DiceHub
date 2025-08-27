import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { LoginComponent } from './page/login.component';
import { LanguageSwitchModule } from '../../shared/components/language-switch/language-switch.module';

@NgModule({
  declarations: [LoginComponent],
  exports: [LoginComponent],
  providers: [],
  imports: [SharedModule, LanguageSwitchModule],
})
export class LoginModule {}
