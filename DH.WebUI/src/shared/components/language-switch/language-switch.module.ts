import { LanguageSwitchComponent } from './language-switch.component';
import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared.module';

@NgModule({
  declarations: [LanguageSwitchComponent],
  exports: [LanguageSwitchComponent],
  imports: [SharedModule],
})
export class LanguageSwitchModule {}
