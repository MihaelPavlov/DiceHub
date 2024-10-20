export interface IUserReward {
  id: number;
  availableMoreForDays: number;
  rewardImageId: number;
  rewardName: string;
  rewardDescription: string;
  status: UserRewardStatus;
}

export enum UserRewardStatus {
  Used = 0,
  Expired = 1,
  NotExpired = 2,
}
