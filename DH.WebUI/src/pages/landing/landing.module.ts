import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { LandingRoutingModule } from './landing-routes.module';
import { LandingComponent } from './page/landing.component';

@NgModule({
  declarations: [LandingComponent],
  exports: [],
  providers: [],
  imports: [SharedModule, LandingRoutingModule],
})
export class LandingModule {}
