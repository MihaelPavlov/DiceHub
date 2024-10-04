import { RewardLevel } from "../enums/reward-level.enum";

export interface IRewardGetByIdResult {
  id: number;
  name: string;
  description: string;
  requiredPoints: number;
  level: RewardLevel;
  imageId: number;
}
