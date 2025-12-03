import { Component } from '@angular/core';

@Component({
    selector: 'app-streak-milestones',
    templateUrl: './streak-rewards.component.html',
    styleUrls: ['./streak-rewards.component.scss'],
    standalone: false
})
export class StreakRewardsComponent {
  milestones = [
    {
      title: 'For 7 Days Streak',
      description: "You'll get instant 10 points to the current active period",
      icon: '/shared/assets/images/streak_reading.png',
      statusIcon: '/shared/assets/images/streak_check.png',
      active: true,
    },
    {
      title: 'For 14 Days Streak',
      description: "You'll get instant 10 points to the current active period",
      icon: '/shared/assets/images/streak_reading.png',
      statusIcon: '/shared/assets/images/streak_check.png',
      active: true,
    },
    {
      title: 'For 21 Days Streak',
      description: "You'll get instant 10 points to the current active period",
      icon: '/shared/assets/images/streak_unlock.png',
      statusIcon: '/shared/assets/images/streak_pending.png',
      active: false,
    },
    {
      title: 'For 28 Days Streak',
      description: 'Bonus 20 points and a special badge',
      icon: '/shared/assets/images/streak_unlock.png',
      statusIcon: '/shared/assets/images/streak_pending_gray.png',
      active: false,
    },
    {
      title: 'For 35 Days Streak',
      description: 'Extra 30 points for dedication!',
      icon: '/shared/assets/images/streak_unlock.png',
      statusIcon: '/shared/assets/images/streak_pending_gray.png',
      active: false,
    },
    {
      title: 'For 42 Days Streak',
      description: 'Surprise reward incoming!',
      icon: '/shared/assets/images/streak_unlock.png',
      statusIcon: '/shared/assets/images/streak_pending_gray.png',
      active: false,
    },
    {
      title: 'For 49 Days Streak',
      description: 'Surprise reward incoming!',
      icon: '/shared/assets/images/streak_unlock.png',
      statusIcon: '/shared/assets/images/streak_pending_gray.png',
      active: false,
    },
  ];

  public isUserStreakActive = true;
  public visibleMilestonesCount = 3;

  public get visibleMilestones() {
    return this.milestones.slice(0, this.visibleMilestonesCount);
  }

  public showMore(): void {
    this.visibleMilestonesCount += 3;

    // cap to total milestones
    if (this.visibleMilestonesCount >= this.milestones.length) {
      this.visibleMilestonesCount = this.milestones.length;
    }
  }

  public get canShowMore(): boolean {
    return this.visibleMilestonesCount < this.milestones.length;
  }
}
