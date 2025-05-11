import { NgModule } from '@angular/core';
import { ResetPasswordComponent } from './reset-password.component';
import { SharedModule } from '../../../shared/shared.module';

@NgModule({
  declarations: [ResetPasswordComponent],
  exports: [ResetPasswordComponent],
  providers: [],
  imports: [SharedModule],
})
export class ResetPasswordModule {}
