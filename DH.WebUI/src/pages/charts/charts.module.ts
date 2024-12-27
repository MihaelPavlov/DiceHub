import { NgModule } from '@angular/core';
import { ChartRoutingModule } from './charts.-routes.module';
import { Chart2Component } from './page/chart.component';
import { VisitorsChartComponent } from '../../features/charts/components/visitors-chart/visitors-chart.component';
import { NgSelectModule } from '@ng-select/ng-select';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { ReservationsChartComponent } from '../../features/charts/components/reservations/reservations-chart.component';

@NgModule({
  declarations: [
    Chart2Component,
    VisitorsChartComponent,
    ReservationsChartComponent,
  ],
  exports: [],
  providers: [],
  imports: [
    SharedModule,
    ChartRoutingModule,
    NgSelectModule,
    FormsModule,
    ReactiveFormsModule,
  ],
})
export class ChartAppModule {}
