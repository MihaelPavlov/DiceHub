export interface ICustomPeriod {
  rewards: ICustomPeriodReward[];
  challenges: ICustomPeriodChallenge[];
  universalChallenges: ICustomPeriodUniversalChallenge[];
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

export interface ICustomPeriodUniversalChallenge {
  id: number | null;
  selectedUniversalChallenge: number;
  attempts: number;
  points: number;
  minValue?: number | null;
}
