import { ChallengeType } from './../../../../pages/challenges-management/shared/challenge-type.enum';
import { Component } from '@angular/core';

@Component({
  selector: 'app-admin-challenges',
  templateUrl: 'admin-challenges.component.html',
  styleUrl: 'admin-challenges.component.scss',
})
export class AdminChallengesComponent {
  public currentActiveTab: ChallengeType = ChallengeType.Game;
  public ChallengeType = ChallengeType;
  
  public tabChange(type: ChallengeType): void {
    this.currentActiveTab = type;
  }
}
