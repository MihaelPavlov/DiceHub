import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { Chart2Component } from "./page/chart.component";

const routes: Routes = [
  {
    path: '',
    component: Chart2Component,
    
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ChartRoutingModule {}
