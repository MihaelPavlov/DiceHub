using DH.Domain.Adapters.Statistics.Enums;
using DH.Domain.Enums;
using DH.Domain.Models.SpaceManagementModels.Queries;
using DH.Domain.Models.StatisticsModels.Queries;
using DH.OperationResultCore.Utility;
using static DH.Domain.Adapters.Statistics.StatisticJobQueue;

namespace DH.Domain.Adapters.Statistics.Services;

public interface IStatisticsService
{
    Task ClubActivityDetectedMessage(ClubActivityDetectedJob job);

    Task EventAttendanceDetectedMessage(EventAttendanceDetectedJob job);

    Task ReservationProcessingOutcomeMessage(ReservationProcessingOutcomeJob job);

    Task RewardActionDetectedMessage(RewardActionDetectedJob job);

    Task ChallengeProcessingOutcomeMessage(ChallengeProcessingOutcomeJob job);


    Task GameEngagementDetectedMessage(GameEngagementDetectedJob job);

    Task<OperationResult<GetActivityChartData>> GetActivityChartData(ChartActivityType type, DateTime rangeStart, DateTime? rangeEnd, CancellationToken cancellationToken);

    Task<OperationResult<List<GetChallengeHistoryLogQueryResponse>>> GetChallengeHistoryLogs(ChallengeHistoryLogType type, CancellationToken cancellationToken);

    Task<OperationResult<List<GetCollectedRewardByDatesModel>>> GetCollectedRewardsByDates(string fromDate, string toDate, CancellationToken cancellationToken);

    Task<OperationResult<GetEventAttendanceChartData>> GetEventAttendanceByIds(int[] eventIds, CancellationToken cancellationToken);

    Task<OperationResult<GetEventAttendanceChartData>> GetEventAttendanceChartData(string fromDate, string toDate, CancellationToken cancellationToken);

    Task<OperationResult<GetExpiredCollectedRewardsChartDataModel>> GetExpiredCollectedRewardsChartData(int year, CancellationToken cancellationToken);

    Task<OperationResult<GetReservationChartData>> GetReservationChartData(string fromDate, string toDate, CancellationToken cancellationToken);
    Task<OperationResult<GetGameActivityChartData>> GetGameActivitydData(ChartGameActivityType type, DateTime? rangeStart, DateTime? rangeEnd, CancellationToken cancellationToken);
    Task<OperationResult<GetUsersWhoPlayedGameData>> GetGameActivityUsersData(int gameId, ChartGameActivityType type, DateTime? rangeStart, DateTime? rangeEnd, CancellationToken cancellationToken);

    Task<GetUserStatsQueryModel> GetUserProfileStats(CancellationToken cancellationToken);
    Task<GetOwnerStatsQueryModel> GetOwnerProfileStats(CancellationToken cancellationToken);
}
