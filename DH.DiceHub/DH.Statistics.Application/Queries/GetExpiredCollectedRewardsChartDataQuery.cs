using DH.OperationResultCore.Utility;
using DH.Statistics.Data;
using DH.Statistics.Domain.Models.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DH.Statistics.Application.Queries;

public record GetExpiredCollectedRewardsChartDataQuery(int Year) : IRequest<OperationResult<GetExpiredCollectedRewardsChartDataModel>>;

internal class GetExpiredCollectedRewardsChartDataQueryHandler(IDbContextFactory<StatisticsDbContext> dbContextFactory) : IRequestHandler<GetExpiredCollectedRewardsChartDataQuery, OperationResult<GetExpiredCollectedRewardsChartDataModel>>
{
    readonly IDbContextFactory<StatisticsDbContext> dbContextFactory = dbContextFactory;

    public async Task<OperationResult<GetExpiredCollectedRewardsChartDataModel>> Handle(GetExpiredCollectedRewardsChartDataQuery request, CancellationToken cancellationToken)
    {
        using (var context = await dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            var collectedRewards = await context.RewardHistoryLogs
                  .Where(x => x.IsCollected && x.CollectedDate != null && x.CollectedDate.Value.Year == request.Year)
                  .GroupBy(x => new { x.CollectedDate!.Value.Year, x.CollectedDate.Value.Month })
                  .Select(g => new RewardsStats
                  {
                      Month = g.Key.Month,
                      CountRewards = g.Count()
                  })
                  .ToListAsync(cancellationToken);

            var expiredRewards = await context.RewardHistoryLogs
                .Where(x => x.IsExpired && x.ExpiredDate != null && x.ExpiredDate.Value.Year == request.Year)
                .GroupBy(x => new { x.ExpiredDate!.Value.Year, x.ExpiredDate.Value.Month })
                .Select(g => new RewardsStats
                {
                    Month = g.Key.Month,
                    CountRewards = g.Count()
                })
                .ToListAsync(cancellationToken);

            var allMonths = Enumerable.Range(1, 12).Select(month => new RewardsStats
            {
                Month = month,
                CountRewards = 0
            }).ToList();

            var completeCollectedRewards = allMonths
              .GroupJoin(collectedRewards, m => m.Month, r => r.Month, (m, r) => new { Month = m.Month, CountRewards = r.Sum(x => x.CountRewards) })
              .Select(grp => new RewardsStats { Month = grp.Month, CountRewards = grp.CountRewards })
              .ToList();

            var completeExpiredRewards = allMonths
                .GroupJoin(expiredRewards, m => m.Month, r => r.Month, (m, r) => new { Month = m.Month, CountRewards = r.Sum(x => x.CountRewards) })
                .Select(grp => new RewardsStats { Month = grp.Month, CountRewards = grp.CountRewards })
                .ToList();

            var result = new GetExpiredCollectedRewardsChartDataModel
            {
                Collected = completeCollectedRewards,
                Expired = completeExpiredRewards
            };

            return new OperationResult<GetExpiredCollectedRewardsChartDataModel>(result);
        }
    }
}