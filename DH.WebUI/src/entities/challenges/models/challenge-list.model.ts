import { ChallengeRewardPoint } from "../enums/challenge-reward-point.enum";

export interface IChallengeListResult {
    id: number;
    rewardPoints: ChallengeRewardPoint;
    attempts: number;
    gameImageId: number;
}