import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';
import { AdminChallengesNavigationComponent } from './page/admin-challenges-navigation.component';
import { AdminChallengesRewardsComponent } from '../../features/admin-challenges-management/components/admin-challenges-rewards/admin-challenges-rewards.component';
import { AdminChallengesListComponent } from '../../features/admin-challenges-management/components/admin-challenges-list/admin-challenges-list.component';
import { AdminChallengesHistoryLogComponent } from '../../features/admin-challenges-management/components/admin-challenges-history-log/admin-challenges-history-log.component';
import { AdminChallengesSystemRewardsComponent } from '../../features/admin-challenges-management/components/admin-challenges-system-rewards/admin-challenges-system-rewards.component';

const routes: Routes = [
  {
    path: '',
    component: AdminChallengesNavigationComponent,
    children: [
      {
        path: 'rewards',
        component: AdminChallengesRewardsComponent,
      },
      {
        path: 'list',
        component: AdminChallengesListComponent,
      },
      {
        path: 'history-log',
        component: AdminChallengesHistoryLogComponent,
      },
    ],
  },
  {
    path: 'system-rewards',
    component: AdminChallengesSystemRewardsComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminChallengesManagementRoutingModule {}
