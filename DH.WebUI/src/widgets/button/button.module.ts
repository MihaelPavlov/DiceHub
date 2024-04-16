import { NgModule } from '@angular/core';
import { ButtonComponent } from './page/button.component';
import { SharedModule } from '../../shared/shared.module';

@NgModule({
  declarations: [ButtonComponent],
  imports: [SharedModule],
  exports: [ButtonComponent],
  providers: [],
})
export class ButtonModule {}
