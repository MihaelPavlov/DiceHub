import { ToastType } from './../../../../shared/models/toast.model';
import { StatisticsService } from './../../../../entities/statistics/api/statistics.service';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ControlsMenuComponent } from '../../../../shared/components/menu/controls-menu.component';
import { ToastService } from '../../../../shared/services/toast.service';
import { UsersService } from '../../../../entities/profile/api/user.service';
import { ChallengeLeaderboardType } from '../../../../entities/statistics/enums/challenge-leaderboard-type.enum';
import { combineLatest } from 'rxjs';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';

interface IChallengeLeaderboardData {
  username: string;
  challengeCount: number;
}

@Component({
  selector: 'leaderboard-challenges',
  templateUrl: 'leaderboard-challenges.component.html',
  styleUrl: 'leaderboard-challenges.component.scss',
})
export class LeaderboardChallengesComponent implements OnInit {
  users: any[] = [];
  public ChallengeLeaderboardType = ChallengeLeaderboardType;
  public currentLeaderboardActiveType: ChallengeLeaderboardType =
    ChallengeLeaderboardType.Weekly;
  public challengeLeaderboardData: IChallengeLeaderboardData[] = [];

  constructor(
    private readonly router: Router,
    private readonly toastService: ToastService,
    private readonly statisticsService: StatisticsService,
    private readonly userService: UsersService,
  ) {}
  public ngOnInit(): void {
    this.fetchLeaderboardData(ChallengeLeaderboardType.Weekly);
  }

  public backNavigateBtn(): void {
    this.router.navigateByUrl('profile');
  }

  public showMenu(event: MouseEvent, controlMenu: ControlsMenuComponent): void {
    event.stopPropagation();
    controlMenu.toggleMenu();
  }

  public fetchLeaderboardData(type: ChallengeLeaderboardType): void {
    this.currentLeaderboardActiveType = type;
    combineLatest([
      this.userService.getUserList(),
      this.statisticsService.getChallengeLeaderboard(type),
    ]).subscribe({
      next: ([users, operation]) => {
        if (
          users &&
          operation &&
          operation.success &&
          operation.relatedObject
        ) {
          console.log(users,operation.relatedObject);
          
          this.challengeLeaderboardData = operation.relatedObject.map((x) => ({
            challengeCount: x.challengeCount,
            username:
              users.find((e) => e.id === x.userId)?.userName ||
              'Username not found',
          }));
        }
      },
      error: () => {
        this.toastService.error({
          message: AppToastMessage.SomethingWrong,
          type: ToastType.Error,
        });
      }
    });
  }
}
