import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared.module';
import { ForbiddenComponent } from './forbidden.component';
import { ForbiddenRoutingModule } from './forbidden-routes.module';

@NgModule({
  imports: [ForbiddenRoutingModule,SharedModule],
  declarations: [ForbiddenComponent],
})
export class ForbiddenModule {}
