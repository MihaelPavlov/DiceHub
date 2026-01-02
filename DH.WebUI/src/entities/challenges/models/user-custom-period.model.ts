import { IUserUniversalChallenge } from './user-universal-challenge.model';

export interface IUserCustomPeriod {
  rewards: IUserCustomPeriodReward[];
  challenges: IUserCustomPeriodChallenge[];
  universalChallenges: IUserUniversalChallenge[];
}

export interface IUserCustomPeriodReward {
  rewardImageUrl: string;
  rewardRequiredPoints: number;
  isCompleted: boolean;
}

export interface IUserCustomPeriodChallenge {
  isCompleted: boolean;
  challengeAttempts: number;
  currentAttempts: number;
  rewardPoints: number;
  gameImageUrl: string;
  gameName: string;
}
