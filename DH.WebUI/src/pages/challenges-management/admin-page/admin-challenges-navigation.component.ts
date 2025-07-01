import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AdminChallengesListComponent } from '../../../features/challenges-management/components/admin-challenges-list/admin-challenges-list.component';
import { AdminChallengesHistoryLogComponent } from '../../../features/challenges-management/components/admin-challenges-history-log/admin-challenges-history-log.component';
import { FULL_ROUTE } from '../../../shared/configs/route.config';
import { Column } from '../../../widgets/nav-bar/page/nav-bar.component';
import { AdminChallengesSystemRewardsComponent } from '../../../features/challenges-management/components/admin-challenges-system-rewards/admin-challenges-system-rewards.component';
import { AdminChallengesCustomPeriodComponent } from '../../../features/challenges-management/components/admin-challenges-custom-period/admin-challenges-custom-period.component';

@Component({
  selector: 'app-admin-challenges-navigation',
  templateUrl: 'admin-challenges-navigation.component.html',
  styleUrl: 'admin-challenges-navigation.component.scss',
})
export class AdminChallengesNavigationComponent {
  public ADMIN_CUSTOM_PERIOD = FULL_ROUTE.CHALLENGES.ADMIN_CUSTOM_PERIOD;
  public ADMIN_LIST = FULL_ROUTE.CHALLENGES.ADMIN_LIST;
  public ADMIN_SYSTEM_REWARDS = FULL_ROUTE.CHALLENGES.ADMIN_SYSTEM_REWARDS;

  public columns: Column[] = [
    {
      name: 'Challenges',
      link: this.ADMIN_LIST,
      isActive: this.isActiveLink(this.ADMIN_LIST),
    },
    {
      name: 'Rewards',
      link: this.ADMIN_SYSTEM_REWARDS,
      isActive: this.isActiveLink(this.ADMIN_SYSTEM_REWARDS),
    },
    {
      name: 'Custom Period',
      link: this.ADMIN_CUSTOM_PERIOD,
      isActive: this.isActiveLink(this.ADMIN_CUSTOM_PERIOD),
    },
  ];

  private activeChildComponent!:
    | AdminChallengesListComponent
    | AdminChallengesCustomPeriodComponent
    | AdminChallengesHistoryLogComponent
    | AdminChallengesSystemRewardsComponent;

  constructor(private readonly router: Router) {}

  public isActiveLink(link: string): boolean {
    return this.router.url.includes(link);
  }

  public onActivate(componentRef: any) {
    this.activeChildComponent = componentRef;
  }
}
