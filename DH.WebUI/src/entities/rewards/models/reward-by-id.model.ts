import { RewardLevel } from "../enums/reward-level.enum";
import { RewardRequiredPoint } from "../enums/reward-required-point.enum";

export interface IRewardGetByIdResult {
  id: number;
  name_EN: string;
  name_BG: string;
  description_EN: string;
  description_BG: string;
  cashEquivalent: number;
  requiredPoints: RewardRequiredPoint;
  level: RewardLevel;
  imageUrl: string;
}
