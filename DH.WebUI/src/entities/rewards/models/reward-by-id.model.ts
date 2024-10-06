import { RewardLevel } from "../enums/reward-level.enum";
import { RewardRequiredPoint } from "../enums/reward-required-point.enum";

export interface IRewardGetByIdResult {
  id: number;
  name: string;
  description: string;
  requiredPoints: RewardRequiredPoint;
  level: RewardLevel;
  imageId: number;
}
