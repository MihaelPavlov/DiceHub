export interface GetReservationChartData {
  gameReservationStats: ReservationStats;
  tableReservationStats: ReservationStats;
}

export interface ReservationStats {
  completed: number;
  cancelled: number;
}
