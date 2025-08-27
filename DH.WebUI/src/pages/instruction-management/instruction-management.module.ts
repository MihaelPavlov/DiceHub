import { NgModule } from '@angular/core';
import { InstructionManagementComponent } from './page/instruction-management.component';
import { SharedModule } from '../../shared/shared.module';
import { InstructionManagementRoutingModule } from './instruction-management-routes.module';
import { HeaderModule } from '../../widgets/header/header.module';
import { InstructionLinksComponent } from '../../features/instruction-management/components/instruction-links/instruction-links.component';
import { InstructionComponent } from '../../features/instruction-management/components/instruction/instruction.component';
import { LandingComponent } from '../landing/page/landing.component';
import { LinkInfoComponent } from '../../features/instruction-management/components/link-info/link-info.component';
import { LanguageSwitchModule } from '../../shared/components/language-switch/language-switch.module';

@NgModule({
  declarations: [
    InstructionManagementComponent,
    InstructionLinksComponent,
    LinkInfoComponent,
    InstructionComponent,
    LandingComponent,
  ],
  exports: [],
  providers: [],
  imports: [
    SharedModule,
    InstructionManagementRoutingModule,
    HeaderModule,
    LanguageSwitchModule,
  ],
})
export class InstructionManagementModule {}
