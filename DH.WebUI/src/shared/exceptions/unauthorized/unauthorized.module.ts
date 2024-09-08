import { NgModule } from '@angular/core';
import { UnauthorizedRoutingModule } from './unauthorized-routes.module';
import { UnauthorizedComponent } from './unauthorized.component';
import { SharedModule } from '../../shared.module';

@NgModule({
  imports: [UnauthorizedRoutingModule,SharedModule],
  declarations: [UnauthorizedComponent],
})
export class UnauthorizedModule {}
