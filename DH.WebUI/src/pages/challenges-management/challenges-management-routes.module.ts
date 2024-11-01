import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ChallengesManagementComponent } from './page/challenges-management.component';
import { ChallengesRewardsComponent } from '../../features/challenges-management/components/challenges-rewards/challenges-rewards.component';
import { AdminChallengesHistoryLogComponent } from '../../features/challenges-management/components/admin-challenges-history-log/admin-challenges-history-log.component';
import { AdminChallengesListComponent } from '../../features/challenges-management/components/admin-challenges-list/admin-challenges-list.component';
import { AdminChallengesRewardsComponent } from '../../features/challenges-management/components/admin-challenges-rewards/admin-challenges-rewards.component';
import { AdminChallengesSystemRewardsComponent } from '../../features/challenges-management/components/admin-challenges-system-rewards/admin-challenges-system-rewards.component';
import { AdminChallengesNavigationComponent } from './admin-page/admin-challenges-navigation.component';
import { ChallengeUserAccessGuard } from '../../shared/guards/challenge-user-access.guard';
import { ROUTE } from '../../shared/configs/route.config';
import { ChallengeAdminAccessGuard } from '../../shared/guards/challenge-admin-access.guard';

const routes: Routes = [
  {
    path: ROUTE.CHALLENGES.HOME,
    component: ChallengesManagementComponent,
    canActivate: [ChallengeUserAccessGuard],
  },
  {
    path: ROUTE.CHALLENGES.REWARDS,
    component: ChallengesRewardsComponent,
    canActivate: [ChallengeUserAccessGuard],
  },
  {
    path: ROUTE.CHALLENGES.ADMIN.CORE,
    component: AdminChallengesNavigationComponent,
    canActivate: [ChallengeAdminAccessGuard],
    children: [
      {
        path: ROUTE.CHALLENGES.ADMIN.REWARDS,
        component: AdminChallengesRewardsComponent,
      },
      {
        path: ROUTE.CHALLENGES.ADMIN.LIST,
        component: AdminChallengesListComponent,
      },
      {
        path: ROUTE.CHALLENGES.ADMIN.HISTORY_LOG,
        component: AdminChallengesHistoryLogComponent,
      },
    ],
  },
  {
    path: `${ROUTE.CHALLENGES.ADMIN.CORE}/${ROUTE.CHALLENGES.ADMIN.SYSTEM_REWARDS}`,
    component: AdminChallengesSystemRewardsComponent,
    canActivate: [ChallengeAdminAccessGuard],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ChallengesManagementRoutingModule {}
