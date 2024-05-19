import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { LoginComponent } from './page/login.component';

@NgModule({
  declarations: [LoginComponent],
  exports: [LoginComponent],
  providers: [],
  imports: [SharedModule],
})
export class LoginModule {}
