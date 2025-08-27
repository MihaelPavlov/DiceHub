import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { RegisterComponent } from './page/register.component';
import { LanguageSwitchModule } from '../../shared/components/language-switch/language-switch.module';

@NgModule({
  declarations: [RegisterComponent],
  exports: [RegisterComponent],
  providers: [],
  imports: [SharedModule, LanguageSwitchModule],
})
export class RegisterModule {}
