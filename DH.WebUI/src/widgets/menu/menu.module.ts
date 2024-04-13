import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { MenuComponent } from './page/menu.component';

@NgModule({
  declarations: [MenuComponent],
  imports: [SharedModule],
  exports: [MenuComponent],
  providers: [],
})
export class MenuModule {}
