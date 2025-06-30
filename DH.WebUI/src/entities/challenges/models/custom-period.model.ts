export interface ICustomPeriod {
  rewards: ICustomPeriodReward[];
  challenges: ICustomPeriodChallenge[];
}

export interface ICustomPeriodReward {
  id: number | null;
  selectedReward: number;
  requiredPoints: number;
}

export interface ICustomPeriodChallenge {
  id: number | null;
  selectedGame: number;
  attempts: number;
  points: number;
}
