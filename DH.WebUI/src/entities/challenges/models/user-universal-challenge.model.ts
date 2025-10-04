import { UniversalChallengeType } from "../../../pages/challenges-management/shared/challenge-universal-type.enum";
import { ChallengeRewardPoint } from "../enums/challenge-reward-point.enum";
import { ChallengeStatus } from "../enums/challenge-status.enum";

export interface IUserUniversalChallenge {
  id: number;
  rewardPoints: ChallengeRewardPoint;
  maxAttempts: number;
  currentAttempts: number;
  status: ChallengeStatus;
  type: UniversalChallengeType;

  minValue?: number;
  gameImageId?: number;
  gameName?: string;

  name_EN: string;
  name_BG: string;
  description_EN: string;
  description_BG: string;
}
