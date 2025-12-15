import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { Chart2Component } from './page/chart.component';
import { VisitorsChartComponent } from '../../features/charts/components/visitors-chart/visitors-chart.component';
import { ReservationsChartComponent } from '../../features/charts/components/reservations/reservations-chart.component';
import { CollectedExpiredRewardsChartComponent } from '../../features/charts/components/rewards/collected-expired-rewards/collected-expired-rewards-chart.component';
import { RewardChartsLayoutComponent } from '../../features/charts/components/rewards/rewards-charts-layout/rewards-charts-layout.component';
import { RewardsCollectedChartComponent } from '../../features/charts/components/rewards/rewards-collected/rewards-collected-chart.component';
import { EventAttendanceChartComponent } from '../../features/charts/components/events/event-attendance-chart.component';
import { LeaderboardChallengesComponent } from '../../features/charts/components/challenges/leaderboard-challenges.component';
import { EventAttendanceByEventsChartComponent } from '../../features/charts/components/events/event-attendance-by-ids/event-attendance-by-events-chart.component';
import { EventsChartsLayoutComponent } from '../../features/charts/components/events/events-charts-layout/events-charts-layout.component';
import { GamesChartComponent } from '../../features/charts/components/games/games-chart.component';
import { AuthGuard } from '../../shared/guards/auth.guard';

const routes: Routes = [
  {
    path: '',
    component: Chart2Component,
    canActivate: [AuthGuard],
  },
  {
    path: 'visitors',
    component: VisitorsChartComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'games',
    component: GamesChartComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'reservations',
    component: ReservationsChartComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'rewards',
    component: RewardChartsLayoutComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'rewards/expired-collected',
    component: CollectedExpiredRewardsChartComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'rewards/collected',
    component: RewardsCollectedChartComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'events',
    component: EventsChartsLayoutComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'events/by-dates',
    component: EventAttendanceChartComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'events/by-events',
    component: EventAttendanceByEventsChartComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'challenges/leaderboard',
    component: LeaderboardChallengesComponent,
    canActivate: [AuthGuard],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ChartRoutingModule {}
