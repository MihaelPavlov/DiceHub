import { ChallengeRewardPoint } from '../enums/challenge-reward-point.enum';

export interface IUpdateUniversalChallengeDto {
  id: number;
  attempts: number;
  rewardPoints: ChallengeRewardPoint;
  minValue?: number;
}
