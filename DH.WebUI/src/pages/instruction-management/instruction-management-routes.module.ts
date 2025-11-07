import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { InstructionManagementComponent } from './page/instruction-management.component';
import { InstructionLinksComponent } from '../../features/instruction-management/components/instruction-links/instruction-links.component';
import { InstructionComponent } from '../../features/instruction-management/components/instruction/instruction.component';

const routes: Routes = [
  {
    path: '',
    component: InstructionManagementComponent, // Main wrapper component
  },
  {
    path: ':key/:linkName',
    component: InstructionComponent,
  },
  { path: 'how_to_install', component: InstructionLinksComponent },
  { path: 'notifications', component: InstructionLinksComponent },
  { path: 'reservation', component: InstructionLinksComponent },
  { path: 'events', component: InstructionLinksComponent },
  { path: 'challenges', component: InstructionLinksComponent },
  { path: 'meeples', component: InstructionLinksComponent },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class InstructionManagementRoutingModule {}
