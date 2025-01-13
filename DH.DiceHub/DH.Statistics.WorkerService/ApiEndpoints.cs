namespace DH.Statistics.WorkerService;

public static class ApiEndpoints
{
    public static class Statistics
    {
        public const string CreateClubActivityLog = "statistic/create-club-activity-log";

        public const string CreateEventAttendanceLog = "statistic/create-event-attendance-log";
        public const string RemoveEventAttendanceLog = "statistic/remove-event-attendance-log";

        public const string CreateReservationOutcomeLog = "statistic/create-reservation-outcome-log";

        public const string CreateChallengeOutcomeLog = "statistic/create-challenge-outcome-log";

        public const string CreateRewardHistoryLog = "statistic/create-reward-history-log";
    }
}
