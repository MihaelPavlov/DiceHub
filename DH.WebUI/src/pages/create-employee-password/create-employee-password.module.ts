import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { CreateEmployeePasswordComponent } from './page/create-employee-password.component';
import { LanguageSwitchModule } from '../../shared/components/language-switch/language-switch.module';

@NgModule({
  declarations: [CreateEmployeePasswordComponent],
  exports: [CreateEmployeePasswordComponent],
  providers: [],
  imports: [SharedModule,LanguageSwitchModule],
})
export class CreateEmployeePasswordModule {}
