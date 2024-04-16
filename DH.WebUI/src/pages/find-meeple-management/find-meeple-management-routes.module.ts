import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { FindMeepleManagementComponent } from "./page/find-meeple-management.component";

const routes: Routes = [
    {
      path: 'find',
      component: FindMeepleManagementComponent,
    },
   
  ];
  
  @NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
  })
  export class FindMeepleManagementRoutingModule {}
  