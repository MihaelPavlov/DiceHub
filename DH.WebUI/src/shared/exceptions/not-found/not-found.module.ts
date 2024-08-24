import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared.module';
import { NotFoundComponent } from './not-found.component';
import { NotFoundRoutingModule } from './not-found-routes.module';

@NgModule({
  imports: [NotFoundRoutingModule,SharedModule],
  declarations: [NotFoundComponent],
})
export class NotFoundModule {}
