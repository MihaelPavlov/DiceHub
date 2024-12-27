import { NgModule } from '@angular/core';
import { ChartRoutingModule } from './charts-routes.module';
import { Chart2Component } from './page/chart.component';
import { VisitorsChartComponent } from '../../features/charts/components/visitors-chart/visitors-chart.component';
import { NgSelectModule } from '@ng-select/ng-select';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { ReservationsChartComponent } from '../../features/charts/components/reservations/reservations-chart.component';
import { CollectedExpiredRewardsChartComponent } from '../../features/charts/components/rewards/collected-expired-rewards/collected-expired-rewards-chart.component';
import { RewardChartsLayoutComponent } from '../../features/charts/components/rewards/rewards-charts-layout/rewards-charts-layout.component';

@NgModule({
  declarations: [
    Chart2Component,
    VisitorsChartComponent,
    ReservationsChartComponent,
    CollectedExpiredRewardsChartComponent,
    RewardChartsLayoutComponent
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
