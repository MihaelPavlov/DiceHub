import { Component } from '@angular/core';
import { AuthService } from '../../../../entities/auth/auth.service';

interface IStreakLeaderboardData {
  username: string;
  streakCount: number;
}

@Component({
  selector: 'app-leaderboard-streak',
  templateUrl: 'streak-leaderboard.component.html',
  styleUrl: 'streak-leaderboard.component.scss',
})
export class StreakLeaderboardComponent {
  public streakLeaderboardData: IStreakLeaderboardData[] = [
    { username: 'daily_dominant', streakCount: 78 },
    { username: 'consistent_carl', streakCount: 74 },
    { username: 'habit_hunter', streakCount: 71 },
    { username: 'focus_felix', streakCount: 69 },
    { username: 'routine_rita', streakCount: 65 },
    // { username: 'rap4obg@abv.bg', streakCount: 63 },
    { username: 'streak_samurai', streakCount: 61 },
    { username: 'win_walter', streakCount: 59 },
    { username: 'board_betty', streakCount: 56 },
    { username: 'meeple_maniac', streakCount: 53 },
    { username: 'steady_steve', streakCount: 49 },
    { username: 'timer_tina', streakCount: 46 },
    { username: 'log_master', streakCount: 42 },
    { username: 'game_grinder', streakCount: 40 },
    { username: 'victory_vince', streakCount: 37 },
    { username: 'focus_fran', streakCount: 34 },
    { username: 'never_miss_nick', streakCount: 31 },
    { username: 'dice_diana', streakCount: 28 },
    { username: 'table_tom', streakCount: 25 },
    { username: 'rookie_roller', streakCount: 22 },
  ];

  public currentUserRank: number | null = null;
  public currentUserStreakCountValue: number = 0;

  constructor(private readonly authService: AuthService) {
    const userIndex = this.streakLeaderboardData.findIndex(
      (u) => u.username === this.authService.getUser?.username
    );

    if (userIndex !== -1) {
      this.currentUserRank = userIndex + 1;
      this.currentUserStreakCountValue =
        this.streakLeaderboardData[userIndex].streakCount;
    } else {
      this.currentUserRank = null;
      this.currentUserStreakCountValue = 0;
    }
  }

    public get getCurrentUserRank(): number | null {
    return this.currentUserRank;
  }

  public get getCurrentUserStreakCountValue(): number {
    return this.currentUserStreakCountValue;
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
