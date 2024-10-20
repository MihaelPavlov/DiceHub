import { UserRewardStatus } from './../../../../entities/rewards/models/user-reward.model';
import { Component, OnInit } from '@angular/core';
import { RewardsService } from '../../../../entities/rewards/api/rewards.service';
import { Observable } from 'rxjs';
import { IUserReward } from '../../../../entities/rewards/models/user-reward.model';

@Component({
  selector: 'app-challenges-rewards',
  templateUrl: 'challenges-rewards.component.html',
  styleUrl: 'challenges-rewards.component.scss',
})
export class ChallengesRewardsComponent implements OnInit {
  public userRewards$!: Observable<IUserReward[]>;
  public UserRewardStatus = UserRewardStatus;
  constructor(private readonly rewardsService: RewardsService) {}

  public ngOnInit(): void {
    this.userRewards$ = this.rewardsService.getUserRewardList();
  }
}
