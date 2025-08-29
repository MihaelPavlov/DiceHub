import { ChallengeRewardPoint } from '../enums/challenge-reward-point.enum';
import { ChallengeStatus } from '../enums/challenge-status.enum';

export interface IUserChallenge {
  id: number;
  rewardPoints: ChallengeRewardPoint;
  maxAttempts: number;
  currentAttempts: number;
  status: ChallengeStatus;
  gameImageId: number;
  gameName: string;
}
