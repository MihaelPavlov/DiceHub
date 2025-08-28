import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { ConfirmEmailComponent } from './page/confirm-email.component';

@NgModule({
  declarations: [ConfirmEmailComponent],
  exports: [ConfirmEmailComponent],
  providers: [],
  imports: [SharedModule],
})
export class ConfirmEmailModule {}
