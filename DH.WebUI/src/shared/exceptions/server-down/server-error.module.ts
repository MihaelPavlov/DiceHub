import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared.module';
import { ServerErrorRoutingModule } from './server-error-routes.module';
import { ServerErrorComponent } from './server-error.component';

@NgModule({
  imports: [ServerErrorRoutingModule, SharedModule],
  declarations: [ServerErrorComponent],
})
export class ServerErrorModule {}
