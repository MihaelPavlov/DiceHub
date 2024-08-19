import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { MenuComponent } from './page/menu.component';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [MenuComponent],
  imports: [SharedModule, RouterModule],
  exports: [MenuComponent],
  providers: [],
})
export class MenuModule {}
