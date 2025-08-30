import { ChallengeRewardPoint } from '../enums/challenge-reward-point.enum';

export interface IChallengeResult {
  id: number;
  rewardPoints: ChallengeRewardPoint;
  attempts: number;
  gameId: number;
}
