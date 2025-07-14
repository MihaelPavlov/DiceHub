import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { CreateOwnerPasswordComponent } from './page/create-owner-password.component';

@NgModule({
  declarations: [CreateOwnerPasswordComponent],
  exports: [CreateOwnerPasswordComponent],
  providers: [],
  imports: [SharedModule],
})
export class CreateOwnerPasswordModule {}
