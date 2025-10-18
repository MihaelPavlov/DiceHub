import { Component, EventEmitter, Input, Output } from '@angular/core';

export enum StreakPageType {
  Streak = 'streak',
  Leaderboard = 'leaderboard',
}

export interface StreakReward {
  id: string;
  title: string;
  streakDays: number;
  currentStreak: number;
  isClaimed: boolean;
}

@Component({
  selector: 'app-streak-rewards',
  templateUrl: './streak.component.html',
  styleUrls: ['./streak.component.scss'],
})
export class StreakRewardsComponent {
  @Input() rewards: StreakReward[] = [
    {
      id: 'daily',
      title: 'Daily Streak',
      streakDays: 100,
      currentStreak: 45,
      isClaimed: false,
    },
    {
      id: 'weekly',
      title: 'Weekly Streak',
      streakDays: 52,
      currentStreak: 15,
      isClaimed: false,
    },
    {
      id: 'monthly',
      title: 'Monthly Streak',
      streakDays: 12,
      currentStreak: 7,
      isClaimed: true,
    },
  ];

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
      icon: '/shared/assets/images/streak_unlock.png',
      statusIcon: '/shared/assets/images/streak_pending.png',
      active: false,
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
      statusIcon: '/shared/assets/images/streak_pending.png',
      active: false,
    },
    {
      title: 'For 35 Days Streak',
      description: 'Extra 30 points for dedication!',
      icon: '/shared/assets/images/streak_unlock.png',
      statusIcon: '/shared/assets/images/streak_pending.png',
      active: false,
    },
    {
      title: 'For 42 Days Streak',
      description: 'Surprise reward incoming!',
      icon: '/shared/assets/images/streak_unlock.png',
      statusIcon: '/shared/assets/images/streak_pending.png',
      active: false,
    },
  ];

  @Output() tabChange = new EventEmitter<StreakPageType>();
  public activeTab: StreakPageType = StreakPageType.Streak;
  public readonly StreakPageType = StreakPageType;
  public isUserStreakActive = true;
  // --- NEW LOGIC ---
  public visibleCount = 3;

  get visibleMilestones() {
    return this.milestones.slice(0, this.visibleCount);
  }

  public showMore(): void {
    this.visibleCount += 3;

    // cap to total milestones
    if (this.visibleCount >= this.milestones.length) {
      this.visibleCount = this.milestones.length;
    }
  }

  get canShowMore(): boolean {
    return this.visibleCount < this.milestones.length;
  }

  public switchTab(tab: StreakPageType): void {
    this.activeTab = tab;
    this.tabChange.emit(this.activeTab);
  }
}
