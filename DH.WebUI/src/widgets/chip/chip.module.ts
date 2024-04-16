import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { ChipComponent } from './page/chip.component';

@NgModule({
  declarations: [ChipComponent],
  imports: [SharedModule],
  exports: [ChipComponent],
  providers: [],
})
export class ChipModule {}
