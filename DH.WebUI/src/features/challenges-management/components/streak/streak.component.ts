import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

export enum StreakPageType {
  Streak = 'streak',
  Leaderboard = 'leaderboard',
}

@Component({
    selector: 'app-streak',
    templateUrl: './streak.component.html',
    styleUrls: ['./streak.component.scss'],
    standalone: false
})
export class StreakComponent implements OnInit {
  public activeTab: StreakPageType = StreakPageType.Streak;
  public readonly StreakPageType = StreakPageType;

  constructor(private route: ActivatedRoute) {}

  public ngOnInit(): void {
   this.route.queryParams.subscribe(params => {
      const tab = params['tab'];
      if (tab === 'leaderboard') {
        this.activeTab = StreakPageType.Leaderboard;
      } else {
        this.activeTab = StreakPageType.Streak;
      }
    });
  }
  
  public switchTab(tab: StreakPageType): void {
    this.activeTab = tab;
  }
}
