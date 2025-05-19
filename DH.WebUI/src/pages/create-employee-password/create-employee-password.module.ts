import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { CreateEmployeePasswordComponent } from './page/create-employee-password.component';

@NgModule({
  declarations: [CreateEmployeePasswordComponent],
  exports: [CreateEmployeePasswordComponent],
  providers: [],
  imports: [SharedModule],
})
export class CreateEmployeePasswordModule {}
