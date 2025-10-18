import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ChallengesManagementComponent } from './page/challenges-management.component';
import { ChallengesRewardsComponent } from '../../features/challenges-management/components/challenges-rewards/challenges-rewards.component';
import { AdminChallengesListComponent } from '../../features/challenges-management/components/admin-challenges-list/admin-challenges-list.component';
import { AdminChallengesCustomPeriodComponent } from '../../features/challenges-management/components/admin-challenges-custom-period/admin-challenges-custom-period.component';
import { AdminChallengesSystemRewardsComponent } from '../../features/challenges-management/components/admin-challenges-system-rewards/admin-challenges-system-rewards.component';
import { AdminChallengesNavigationComponent } from './admin-page/admin-challenges-navigation.component';
import { ChallengeUserAccessGuard } from '../../shared/guards/challenge-user-access.guard';
import { ROUTE } from '../../shared/configs/route.config';
import { ChallengeAdminAccessGuard } from '../../shared/guards/challenge-admin-access.guard';
import { canDeactivateGuard } from '../../shared/guards/can-deactive.guard';
import { StreakRewardsComponent } from '../../features/challenges-management/components/streak/streak.component';

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
    path: ROUTE.CHALLENGES.STREAKS,
    component: StreakRewardsComponent,
    canActivate: [ChallengeUserAccessGuard],
  },
  {
    path: ROUTE.CHALLENGES.ADMIN.CORE,
    component: AdminChallengesNavigationComponent,
    canActivate: [ChallengeAdminAccessGuard],
    children: [
      {
        path: ROUTE.CHALLENGES.ADMIN.CUSTOM_PERIOD,
        component: AdminChallengesCustomPeriodComponent,
        canDeactivate:[canDeactivateGuard]
      },
      {
        path: ROUTE.CHALLENGES.ADMIN.LIST,
        component: AdminChallengesListComponent,
      },
      {
        path: `${ROUTE.CHALLENGES.ADMIN.SYSTEM_REWARDS}`,
        component: AdminChallengesSystemRewardsComponent,
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ChallengesManagementRoutingModule {}
