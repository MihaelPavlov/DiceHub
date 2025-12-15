import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ChallengesManagementComponent } from './page/challenges-management.component';
import { ChallengesRewardsComponent } from '../../features/challenges-management/components/challenges-rewards/challenges-rewards.component';
import { AdminChallengesCustomPeriodComponent } from '../../features/challenges-management/components/admin-challenges-custom-period/admin-challenges-custom-period.component';
import { AdminChallengesSystemRewardsComponent } from '../../features/challenges-management/components/admin-challenges-system-rewards/admin-challenges-system-rewards.component';
import { AdminChallengesNavigationComponent } from './admin-page/admin-challenges-navigation.component';
import { ChallengeUserAccessGuard } from '../../shared/guards/challenge-user-access.guard';
import { ROUTE } from '../../shared/configs/route.config';
import { ChallengeAdminAccessGuard } from '../../shared/guards/challenge-admin-access.guard';
import { canDeactivateGuard } from '../../shared/guards/can-deactive.guard';
import { AdminChallengesComponent } from '../../features/challenges-management/components/admin-challenges/admin-challenges.component';
import { AuthGuard } from '../../shared/guards/auth.guard';

const routes: Routes = [
  {
    path: ROUTE.CHALLENGES.HOME,
    component: ChallengesManagementComponent,
    canActivate: [AuthGuard, ChallengeUserAccessGuard],
  },
  {
    path: ROUTE.CHALLENGES.REWARDS,
    component: ChallengesRewardsComponent,
    canActivate: [AuthGuard, ChallengeUserAccessGuard],
  },
  // FUTURE Feature - Streaks Page
  // {
  //   path: ROUTE.CHALLENGES.STREAKS,
  //   component: StreakComponent,
  //   canActivate: [AuthGuard,ChallengeUserAccessGuard],
  // },
  {
    path: ROUTE.CHALLENGES.ADMIN.CORE,
    component: AdminChallengesNavigationComponent,
    canActivate: [AuthGuard, ChallengeAdminAccessGuard],
    children: [
      {
        path: ROUTE.CHALLENGES.ADMIN.CUSTOM_PERIOD,
        component: AdminChallengesCustomPeriodComponent,
        canActivate: [AuthGuard],
        canDeactivate: [canDeactivateGuard],
      },
      {
        path: ROUTE.CHALLENGES.ADMIN.LIST,
        component: AdminChallengesComponent,
        canActivate: [AuthGuard],
      },
      {
        path: `${ROUTE.CHALLENGES.ADMIN.SYSTEM_REWARDS}`,
        component: AdminChallengesSystemRewardsComponent,
        canActivate: [AuthGuard],
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ChallengesManagementRoutingModule {}
