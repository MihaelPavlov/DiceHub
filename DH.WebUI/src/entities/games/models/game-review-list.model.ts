export interface IGameReviewListResult {
  id: number;
  gameId: number;
  userId: string;
  userImageUrl: string;
  userFullName: string;
  review: string;
  updatedDate: Date;
}
