import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { InstructionManagementComponent } from './page/instruction-management.component';

const routes: Routes = [
  {
    path: '',
    component: InstructionManagementComponent,
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class InstructionManagementRoutingModule {}
