import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { ConfirmEmailComponent } from './page/confirm-email.component';
import { LanguageSwitchModule } from '../../shared/components/language-switch/language-switch.module';

@NgModule({
  declarations: [ConfirmEmailComponent],
  exports: [ConfirmEmailComponent],
  providers: [],
  imports: [SharedModule, LanguageSwitchModule],
})
export class ConfirmEmailModule {}
