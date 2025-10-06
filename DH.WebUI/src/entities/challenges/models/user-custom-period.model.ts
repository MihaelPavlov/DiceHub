import { IUserUniversalChallenge } from './user-universal-challenge.model';

export interface IUserCustomPeriod {
  rewards: IUserCustomPeriodReward[];
  challenges: IUserCustomPeriodChallenge[];
  universalChallenges: IUserUniversalChallenge[];
}

export interface IUserCustomPeriodReward {
  rewardImageId: number;
  rewardRequiredPoints: number;
  isCompleted: boolean;
}

export interface IUserCustomPeriodChallenge {
  isCompleted: boolean;
  challengeAttempts: number;
  currentAttempts: number;
  rewardPoints: number;
  gameImageId: number;
  gameName: string;
}
