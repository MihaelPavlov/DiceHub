import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { ResetPasswordComponent } from './page/reset-password.component';

@NgModule({
  declarations: [ResetPasswordComponent],
  exports: [ResetPasswordComponent],
  providers: [],
  imports: [SharedModule],
})
export class ResetPasswordModule {}
