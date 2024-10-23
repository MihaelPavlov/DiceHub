import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { RegisterComponent } from './page/register.component';

@NgModule({
  declarations: [RegisterComponent],
  exports: [RegisterComponent],
  providers: [],
  imports: [SharedModule],
})
export class RegisterModule {}
