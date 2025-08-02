export interface GetGameActivityChartData {
  games: GameActivityStats[];
}

export interface GameActivityStats {
  gameId: number;
  gameName: string;
  timesPlayed: number;
  gameImageId: number;
}
