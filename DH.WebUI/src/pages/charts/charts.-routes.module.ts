import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { Chart2Component } from './page/chart.component';
import { VisitorsChartComponent } from '../../features/charts/components/visitors-chart/visitors-chart.component';
import { ReservationsChartComponent } from '../../features/charts/components/reservations/reservations-chart.component';

const routes: Routes = [
  {
    path: '',
    component: Chart2Component,
  },
  {
    path: 'visitors',
    component: VisitorsChartComponent,
  },
  {
    path: 'reservations',
    component: ReservationsChartComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ChartRoutingModule {}
