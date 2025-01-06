export interface GetEventAttendanceChartData {
  eventAttendances: EventAttendance[];
}

export interface EventAttendance {
  userAttendedCount: number;
  eventId: number;
}
