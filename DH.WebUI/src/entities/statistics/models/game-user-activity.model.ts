export interface GetUsersWhoPlayedGameData {
  users: GameUserActivity[];
}

export interface GameUserActivity {
  userId: string;
  userDisplayName: string;
  playedAt: Date;
}
