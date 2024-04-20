import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ChallengesManagementComponent } from './page/challenges-management.component';
import { ChallengesRewardsComponent } from '../../features/challenges-management/components/challenges-rewards/challenges-rewards.component';

const routes: Routes = [
  {
    path: 'home',
    component: ChallengesManagementComponent,
  },
  {
    path: 'rewards',
    component: ChallengesRewardsComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ChallengesManagementRoutingModule {}
