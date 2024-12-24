import { NgModule } from '@angular/core';
import { ChartRoutingModule } from './charts.-routes.module';
import { Chart2Component } from './page/chart.component';

@NgModule({
  declarations: [Chart2Component],
  exports: [],
  providers: [],
  imports: [ChartRoutingModule],
})
export class ChartAppModule {}
