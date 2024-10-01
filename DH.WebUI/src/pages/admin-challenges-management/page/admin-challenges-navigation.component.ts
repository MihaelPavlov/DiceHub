import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { IMenuItem } from '../../../shared/models/menu-item.model';
import { AdminChallengesRewardsComponent } from '../../../features/admin-challenges-management/components/admin-challenges-rewards/admin-challenges-rewards.component';
import { AdminChallengesListComponent } from '../../../features/admin-challenges-management/components/admin-challenges-list/admin-challenges-list.component';
import { AdminChallengesHistoryLogComponent } from '../../../features/admin-challenges-management/components/admin-challenges-history-log/admin-challenges-history-log.component';

@Component({
  selector: 'app-admin-challenges-navigation',
  templateUrl: 'admin-challenges-navigation.component.html',
  styleUrl: 'admin-challenges-navigation.component.scss',
})
export class AdminChallengesNavigationComponent {
  public menuItems: IMenuItem[] = [];

  private activeChildComponent!:
    | AdminChallengesListComponent
    | AdminChallengesRewardsComponent
    | AdminChallengesHistoryLogComponent;

  constructor(private readonly router: Router) {
    this.handleMenuItemClick = this.handleMenuItemClick.bind(this);
  }

  public ngOnInit(): void {
    this.menuItems = [
      { key: 'settings', label: 'Settings' },
      { key: 'system-rewards', label: 'System Rewards' },
      { key: 'custom-challenges', label: 'Custom Challenges' },
    ];
  }

  public handleMenuItemClick(key: string): void {
    //TODO: CHANGE THE URLS
    if (key === 'settings') {
      this.router.navigateByUrl('/admin-challenges/settings');
    } else if (key === 'system-rewards') {
      this.router.navigateByUrl('/admin-challenges/system-rewards');
    } else if (key === 'custom-challenges') {
      this.router.navigateByUrl('/admin-challenges/history-log');
    }
  }

  public isActiveLink(link: string): boolean {
    return this.router.url.includes(link);
  }

  public onActivate(componentRef: any) {
    this.activeChildComponent = componentRef;
  }
}
