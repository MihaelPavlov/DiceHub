import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { InstructionManagementComponent } from './page/instruction-management.component';
import { InstructionLinksComponent } from '../../features/instruction-management/components/instruction-links/instruction-links.component';

const routes: Routes = [
  {
    path: '',
    component: InstructionManagementComponent, // Main wrapper component
  },
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
