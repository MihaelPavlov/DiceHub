import { RewardLevel } from '../enums/reward-level.enum';
import { RewardRequiredPoint } from '../enums/reward-required-point.enum';

export interface ICreateRewardDto {
  name: string;
  cashEquivalent: number;
  description: string;
  requiredPoints: RewardRequiredPoint;
  level: RewardLevel;
}
