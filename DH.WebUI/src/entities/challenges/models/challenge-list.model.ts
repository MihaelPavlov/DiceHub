import { ChallengeRewardPoint } from "../enums/challenge-reward-point.enum";
import { ChallengeType } from "../enums/challenge-type.enum";

export interface IChallengeListResult {
    id: number;
    description: string;
    rewardPoints: ChallengeRewardPoint;
    attempts: number;
    gameImageId: number;
    type: ChallengeType
}