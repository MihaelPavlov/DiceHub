import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { IMenuItem } from '../../../shared/models/menu-item.model';
import { AdminChallengesRewardsComponent } from '../../../features/challenges-management/components/admin-challenges-rewards/admin-challenges-rewards.component';
import { AdminChallengesListComponent } from '../../../features/challenges-management/components/admin-challenges-list/admin-challenges-list.component';
import { AdminChallengesHistoryLogComponent } from '../../../features/challenges-management/components/admin-challenges-history-log/admin-challenges-history-log.component';
import { FULL_ROUTE } from '../../../shared/configs/route.config';
import { Column } from '../../../widgets/nav-bar/page/nav-bar.component';
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-admin-challenges-navigation',
  templateUrl: 'admin-challenges-navigation.component.html',
  styleUrl: 'admin-challenges-navigation.component.scss',
})
export class AdminChallengesNavigationComponent {
  public menuItems: BehaviorSubject<IMenuItem[]> = new BehaviorSubject<
    IMenuItem[]
  >([]);
  public ADMIN_REWARDS = FULL_ROUTE.CHALLENGES.ADMIN_REWARDS;
  public ADMIN_LIST = FULL_ROUTE.CHALLENGES.ADMIN_LIST;
  public ADMIN_HISTORY_LOG = FULL_ROUTE.CHALLENGES.ADMIN_HISTORY_LOG;

  // TODO: Update the word weekly based on the settings. Weekly rewards could be monthly
  public columns: Column[] = [
    {
      name: 'Weekly Rewards',
      link: this.ADMIN_REWARDS,
      isActive: this.isActiveLink(this.ADMIN_REWARDS),
    },
    {
      name: 'Challenges',
      link: this.ADMIN_LIST,
      isActive: this.isActiveLink(this.ADMIN_LIST),
    },
    {
      name: 'History Log',
      link: this.ADMIN_HISTORY_LOG,
      isActive: this.isActiveLink(this.ADMIN_HISTORY_LOG),
    },
  ];

  private activeChildComponent!:
    | AdminChallengesListComponent
    | AdminChallengesRewardsComponent
    | AdminChallengesHistoryLogComponent;

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
