export interface IUserReward {
  id: number;
  availableMoreForDays: number;
  rewardImageUrl: string;
  rewardName_EN: string;
  rewardName_BG: string;
  rewardDescription_EN: string;
  rewardDescription_BG: string;
  status: UserRewardStatus;
}

export enum UserRewardStatus {
  Used = 0,
  Expired = 1,
  NotExpired = 2,
}
