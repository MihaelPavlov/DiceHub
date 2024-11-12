import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { NavigationMenuComponent } from './page/navigation-menu.component';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [NavigationMenuComponent],
  imports: [SharedModule, RouterModule],
  exports: [NavigationMenuComponent],
  providers: [],
})
export class NavigationMenuModule {}
