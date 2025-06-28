import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { IMenuItem } from '../../../shared/models/menu-item.model';
import { AdminChallengesListComponent } from '../../../features/challenges-management/components/admin-challenges-list/admin-challenges-list.component';
import { AdminChallengesHistoryLogComponent } from '../../../features/challenges-management/components/admin-challenges-history-log/admin-challenges-history-log.component';
import { FULL_ROUTE } from '../../../shared/configs/route.config';
import { Column } from '../../../widgets/nav-bar/page/nav-bar.component';
import { BehaviorSubject } from 'rxjs';
import { AdminChallengesSystemRewardsComponent } from '../../../features/challenges-management/components/admin-challenges-system-rewards/admin-challenges-system-rewards.component';
import { AdminChallengesCustomPeriodComponent } from '../../../features/challenges-management/components/admin-challenges-custom-period/admin-challenges-custom-period.component';

@Component({
  selector: 'app-admin-challenges-navigation',
  templateUrl: 'admin-challenges-navigation.component.html',
  styleUrl: 'admin-challenges-navigation.component.scss',
})
export class AdminChallengesNavigationComponent {
  public menuItems: BehaviorSubject<IMenuItem[]> = new BehaviorSubject<
    IMenuItem[]
  >([]);
  public ADMIN_CUSTOM_PERIOD= FULL_ROUTE.CHALLENGES.ADMIN_CUSTOM_PERIOD;
  public ADMIN_LIST = FULL_ROUTE.CHALLENGES.ADMIN_LIST;
  public ADMIN_SYSTEM_REWARDS = FULL_ROUTE.CHALLENGES.ADMIN_SYSTEM_REWARDS;
  public ADMIN_HISTORY_LOG = FULL_ROUTE.CHALLENGES.ADMIN_HISTORY_LOG;

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
    // {
    //   name: 'History Log',
    //   link: this.ADMIN_HISTORY_LOG,
    //   isActive: this.isActiveLink(this.ADMIN_HISTORY_LOG),
    // },
  ];

  private activeChildComponent!:
    | AdminChallengesListComponent
    | AdminChallengesCustomPeriodComponent
    | AdminChallengesHistoryLogComponent
    | AdminChallengesSystemRewardsComponent;

  constructor(private readonly router: Router) {
    this.handleMenuItemClick = this.handleMenuItemClick.bind(this);
  }

  public ngOnInit(): void {
    this.menuItems.next([
      { key: 'settings', label: 'Settings' },
      { key: 'system-rewards', label: 'System Rewards' },
      { key: 'custom-challenges', label: 'Custom Challenges' },
    ]);
  }

  public handleMenuItemClick(key: string): void {
    //TODO: CHANGE THE URLS
    if (key === 'settings') {
      this.router.navigateByUrl(FULL_ROUTE.CHALLENGES.ADMIN_SETTINGS);
    } else if (key === 'system-rewards') {
      this.router.navigateByUrl(FULL_ROUTE.CHALLENGES.ADMIN_SYSTEM_REWARDS);
    } else if (key === 'custom-challenges') {
      this.router.navigateByUrl(FULL_ROUTE.CHALLENGES.ADMIN_HISTORY_LOG);
    }
  }

  public isActiveLink(link: string): boolean {
    return this.router.url.includes(link);
  }

  public onActivate(componentRef: any) {
    this.activeChildComponent = componentRef;
  }
}
