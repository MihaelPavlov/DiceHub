import { ChallengeRewardPoint } from '../enums/challenge-reward-point.enum';
import { ChallengeType } from '../enums/challenge-type.enum';

export interface IChallengeResult {
  id: number;
  rewardPoints: ChallengeRewardPoint;
  attempts: number;
  type: ChallengeType;
  gameId: number;
}
