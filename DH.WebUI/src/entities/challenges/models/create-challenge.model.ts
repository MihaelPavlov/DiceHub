import { ChallengeRewardPoint } from "../enums/challenge-reward-point.enum";
import { ChallengeType } from "../enums/challenge-type.enum";

export interface ICreateChallengeDto {
    rewardPoints: ChallengeRewardPoint;
    attempts: number;
    type: ChallengeType;
    gameId: number;
  }