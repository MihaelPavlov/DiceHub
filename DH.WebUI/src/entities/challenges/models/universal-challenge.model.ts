import { UniversalChallengeType } from '../../../pages/challenges-management/shared/challenge-universal-type.enum';
import { ChallengeRewardPoint } from '../enums/challenge-reward-point.enum';

export interface IUniversalChallengeListResult {
  id: number;
  rewardPoints: ChallengeRewardPoint;
  name_EN: string;
  name_BG: string;
  description_EN: string;
  description_BG: string;
  type: UniversalChallengeType;

  attempts?: number;
  minValue?: number;

  showDescription?: boolean;
}
