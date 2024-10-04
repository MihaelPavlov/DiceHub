import { RewardLevel } from '../enums/reward-level.enum';

export interface IUpdateRewardDto {
  id: number;
  name: string;
  description: string;
  requiredPoints: number;
  level: RewardLevel;
  imageId: number | null;
}
