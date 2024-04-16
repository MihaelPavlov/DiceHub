import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { HeaderModule } from '../../widgets/header/header.module';
import { FindMeepleManagementRoutingModule } from './find-meeple-management-routes.module';
import { FindMeepleManagementComponent } from './page/find-meeple-management.component';

@NgModule({
  declarations: [FindMeepleManagementComponent],
  exports: [FindMeepleManagementComponent],
  providers: [],
  imports: [SharedModule, HeaderModule, FindMeepleManagementRoutingModule],
})
export class FindMeepleMamagementModule {}
