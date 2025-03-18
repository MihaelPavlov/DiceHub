import { NgModule } from '@angular/core';
import { InstructionManagementComponent } from './page/instruction-management.component';
import { SharedModule } from '../../shared/shared.module';
import { InstructionManagementRoutingModule } from './instruction-management-routes.module';
import { HeaderModule } from '../../widgets/header/header.module';

@NgModule({
  declarations: [InstructionManagementComponent],
  exports: [InstructionManagementComponent],
  providers: [],
  imports: [SharedModule, HeaderModule],
})
export class InstructionManagementModule {}
