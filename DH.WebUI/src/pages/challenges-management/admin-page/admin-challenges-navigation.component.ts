import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AdminChallengesListComponent } from '../../../features/challenges-management/components/admin-challenges-list/admin-challenges-list.component';
import { AdminChallengesHistoryLogComponent } from '../../../features/challenges-management/components/admin-challenges-history-log/admin-challenges-history-log.component';
import { FULL_ROUTE } from '../../../shared/configs/route.config';
import { Column } from '../../../widgets/nav-bar/page/nav-bar.component';
import { AdminChallengesSystemRewardsComponent } from '../../../features/challenges-management/components/admin-challenges-system-rewards/admin-challenges-system-rewards.component';
import { AdminChallengesCustomPeriodComponent } from '../../../features/challenges-management/components/admin-challenges-custom-period/admin-challenges-custom-period.component';
import { TranslateService } from '@ngx-translate/core';
import { BehaviorSubject } from 'rxjs';
import { IMenuItem } from '../../../shared/models/menu-item.model';

@Component({
  selector: 'app-admin-challenges-navigation',
  templateUrl: 'admin-challenges-navigation.component.html',
  styleUrl: 'admin-challenges-navigation.component.scss',
})
export class AdminChallengesNavigationComponent implements OnInit {
  public ADMIN_CUSTOM_PERIOD = FULL_ROUTE.CHALLENGES.ADMIN_CUSTOM_PERIOD;
  public ADMIN_LIST = FULL_ROUTE.CHALLENGES.ADMIN_LIST;
  public ADMIN_SYSTEM_REWARDS = FULL_ROUTE.CHALLENGES.ADMIN_SYSTEM_REWARDS;

  public menuItems: BehaviorSubject<IMenuItem[]> = new BehaviorSubject<
    IMenuItem[]
  >([]);

  public columns: Column[] = [
    {
      name: this.translateService.instant('admin_challenge.columns.challenges'),
      link: this.ADMIN_LIST,
      isActive: this.isActiveLink(this.ADMIN_LIST),
    },
    {
      name: this.translateService.instant('admin_challenge.columns.rewards'),
      link: this.ADMIN_SYSTEM_REWARDS,
      isActive: this.isActiveLink(this.ADMIN_SYSTEM_REWARDS),
    },
    {
      name: this.translateService.instant(
        'admin_challenge.columns.custom_period'
      ),
      link: this.ADMIN_CUSTOM_PERIOD,
      isActive: this.isActiveLink(this.ADMIN_CUSTOM_PERIOD),
    },
  ];

  private activeChildComponent!:
    | AdminChallengesListComponent
    | AdminChallengesCustomPeriodComponent
    | AdminChallengesHistoryLogComponent
    | AdminChallengesSystemRewardsComponent;

  constructor(
    private readonly router: Router,
    private readonly translateService: TranslateService
  ) {}

  public ngOnInit(): void {
    this.menuItems.next([
      { key: 'add-game', label: 'Generic Challenges' }, 
      // Play X Games, 
      // Join Or Create X Meeple Rooms,
      // Join X Events,
      // Use X Rewards,
      // Buy X Items above x value/money$
      // Play 1 time Favorite Game, 
      // Stay 2/3 days at top 3 of the Challenge Leaderboard, 
      // Stay 2/3 days in Top 3 streak leaderboard
      {
        key: 'add-existing-game',
        label: 'Streaks',
      },
    ]);
  }

  public isActiveLink(link: string): boolean {
    return this.router.url.includes(link);
  }

  public onActivate(componentRef: any) {
    this.activeChildComponent = componentRef;
  }

  public handleMenuItemClick(key: string): void {
    if (key === 'add-game') {
      this.router.navigateByUrl('/games/add');
    } else if (key === 'add-existing-game') {
      this.router.navigateByUrl('/games/add-existing-game');
    }
  }
}
