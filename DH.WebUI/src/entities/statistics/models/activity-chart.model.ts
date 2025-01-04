export interface GetActivityChartData {
    logs: ActivityLog[];
}

export interface ActivityLog {
    userCount: number;
    date: Date;
}