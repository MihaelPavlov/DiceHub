import {
  Component,
  OnInit
} from '@angular/core';
import { Router } from '@angular/router';
import { ChallengeLeaderboardType } from '../../../../entities/statistics/enums/challenge-leaderboard-type.enum';
import { ROUTE } from '../../../../shared/configs/route.config';
import { UsersService } from '../../../../entities/profile/api/user.service';
import { StatisticsService } from '../../../../entities/statistics/api/statistics.service';
import { ToastService } from '../../../../shared/services/toast.service';
import { AuthService } from '../../../../entities/auth/auth.service';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../../shared/models/toast.model';
import { combineLatest } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';

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
  public currentUserRank: number | null = null;
  public currentUserChallengeCountValue: number = 0;
  constructor(
    private readonly router: Router,
    private readonly toastService: ToastService,
    private readonly statisticsService: StatisticsService,
    private readonly userService: UsersService,
    private readonly authService: AuthService,
    private readonly translateService: TranslateService
  ) {}

  public ngOnInit(): void {
    this.fetchLeaderboardData(ChallengeLeaderboardType.Weekly);
  }

  public backNavigateBtn(): void {
    this.router.navigateByUrl(ROUTE.PROFILE.CORE);
  }

  public get getCurrentUserRank(): number | null {
    return this.currentUserRank;
  }

  public get getCurrentUserChallengeCountValue(): number {
    return this.currentUserChallengeCountValue;
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
          console.log(users, operation.relatedObject);

          this.challengeLeaderboardData = operation.relatedObject.map((x) => ({
            challengeCount: x.challengeCount,
            username:
              users.find((e) => e.id === x.userId)?.userName ||
              this.translateService.instant(
                'challenge_leaderboard.username_not_found'
              ),
          }));

          const userIndex = this.challengeLeaderboardData.findIndex(
            (u) => u.username === this.authService.getUser?.username
          );

          if (userIndex !== -1) {
            this.currentUserRank = userIndex + 1;
            this.currentUserChallengeCountValue =
              this.challengeLeaderboardData[userIndex].challengeCount;
          } else {
            this.currentUserRank = null;
            this.currentUserChallengeCountValue = 0;
          }
        }
      },
      error: () => {
        this.toastService.error({
          message: this.translateService.instant(
            AppToastMessage.SomethingWrong
          ),
          type: ToastType.Error,
        });
      },
    });
  }

  public scrollToCurrentUser(): void {
    const currentUserRank = this.authService.getUser?.username; // you can dynamically determine this
    const elementId = 'user-rank-' + currentUserRank;
    const element = document.getElementById(elementId);

    if (element) {
      element.scrollIntoView({
        behavior: 'smooth',
        block: 'center',
      });

      // Add breathing effect class
      element.classList.add('breathing-highlight');

      // Remove it after 3 seconds
      setTimeout(() => {
        element.classList.remove('breathing-highlight');
      }, 3000);
    }
  }
}
