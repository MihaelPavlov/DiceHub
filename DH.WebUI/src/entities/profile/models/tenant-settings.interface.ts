export interface TenantSetting {
  id: number;

  /**
   * Used to have the average capacity of the place
   */
  averageMaxCapacity: number;

  /**
   * The rewards that the user can receive in specific time period
   */
  challengeRewardsCountForPeriod: number;

  /**
   * Period of the rewards reset. Weekly, Monthly, etc.
   */
  periodOfRewardReset: string;

  /**
   * The day at 12:00 PM when rewards will be reset
   */
  resetDayForRewards: string;

  /**
   * Defines the number of hours to delay the initiation of the new challenge
   */
  challengeInitiationDelayHours: number;
}
