import { ChallengeRewardPoint } from "../enums/challenge-reward-point.enum";

export interface ICreateChallengeDto {
    rewardPoints: ChallengeRewardPoint;
    attempts: number;
    gameId: number;
  }