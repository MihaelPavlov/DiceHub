export interface GetExpiredCollectedRewardsChart {
  expired: RewardsStats[];
  collected: RewardsStats[];
}

export interface RewardsStats {
  month: number;
  countRewards: number;
  totalCashEquivalent: number;
}
