import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { ForgotPasswordComponent } from './page/forgot-password.component';

@NgModule({
  declarations: [ForgotPasswordComponent],
  exports: [ForgotPasswordComponent],
  providers: [],
  imports: [SharedModule],
})
export class ForgotPasswordModule {}
