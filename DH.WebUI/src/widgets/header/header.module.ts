import { NgModule } from '@angular/core';
import { HeaderComponent } from './page/header.component';
import { SharedModule } from '../../shared/shared.module';

@NgModule({
  declarations: [HeaderComponent],
  exports: [HeaderComponent],
  providers: [],
  imports: [SharedModule],
})
export class HeaderModule {}
