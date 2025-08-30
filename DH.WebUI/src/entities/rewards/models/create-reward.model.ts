import { RewardLevel } from '../enums/reward-level.enum';
import { RewardRequiredPoint } from '../enums/reward-required-point.enum';

export interface ICreateRewardDto {
  name_EN: string;
  name_BG: string;
  cashEquivalent: number;
  description_EN: string;
  description_BG: string;
  requiredPoints: RewardRequiredPoint;
  level: RewardLevel;
}
