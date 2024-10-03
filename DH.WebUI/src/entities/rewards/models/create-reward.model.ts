import { RewardLevel } from '../enums/reward-level.enum';

export interface ICreateRewardDto {
  name: string;
  description: string;
  requiredPoints: number;
  level: RewardLevel;
}
