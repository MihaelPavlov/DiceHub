import { NgModule } from '@angular/core';
import { InstructionManagementComponent } from './page/instruction-management.component';
import { SharedModule } from '../../shared/shared.module';
import { InstructionManagementRoutingModule } from './instruction-management-routes.module';
import { HeaderModule } from '../../widgets/header/header.module';
import { InstructionLinksComponent } from '../../features/instruction-management/components/instruction-links/instruction-links.component';

@NgModule({
  declarations: [InstructionManagementComponent, InstructionLinksComponent],
  exports: [],
  providers: [],
  imports: [SharedModule, InstructionManagementRoutingModule, HeaderModule],
})
export class InstructionManagementModule {}
