import { RewardLevel } from "./reward-level.enum";

export enum RewardRequiredPoint {
  Five = 5,
  Ten = 10,
  Fifteen = 15,
  Twenty = 20,
  TwentyFive = 25,
  Thirty = 30,
  ThirtyFive = 35,
  Forty = 40,
  FortyFive = 45,
  Fifty = 50,
  FiftyFive = 55,
  Sixty = 60,
  SixtyFive = 65,
  Seventy = 70,
  SeventyFive = 75,
  Eighty = 80,
  EightyFive = 85,
  Ninety = 90,
  NinetyFive = 95,
  OneHundred = 100,
}

export const REWARD_POINTS = {
  [RewardLevel.Bronze]: [RewardRequiredPoint.Five,RewardRequiredPoint.Ten, RewardRequiredPoint.Fifteen, RewardRequiredPoint.Twenty],
  [RewardLevel.Silver]: [RewardRequiredPoint.TwentyFive, RewardRequiredPoint.Thirty, RewardRequiredPoint.ThirtyFive, RewardRequiredPoint.Forty],
  [RewardLevel.Gold]: [RewardRequiredPoint.FortyFive, RewardRequiredPoint.Fifty, RewardRequiredPoint.FiftyFive, RewardRequiredPoint.Sixty, RewardRequiredPoint.SixtyFive, RewardRequiredPoint.Seventy, RewardRequiredPoint.SeventyFive],
  [RewardLevel.Platinum]: [RewardRequiredPoint.Eighty, RewardRequiredPoint.EightyFive, RewardRequiredPoint.Ninety, RewardRequiredPoint.NinetyFive, RewardRequiredPoint.OneHundred]
};
