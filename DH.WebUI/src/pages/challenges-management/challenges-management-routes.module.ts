import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { ChallengesManagementComponent } from "./page/challenges-management.component";

const routes: Routes = [
    {
      path: 'home',
      component: ChallengesManagementComponent,
    },
 
  ];
  
  @NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
  })
  export class ChallengesManagementRoutingModule {}
  