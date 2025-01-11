using DH.OperationResultCore.Extension;
using DH.OperationResultCore.Utility;
using DH.Statistics.Application.Helpers;
using DH.Statistics.Data;
using DH.Statistics.Domain.Models.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DH.Statistics.Application.Queries;

public record GetCollectedRewardsByDatesQuery(string FromDate, string ToDate) : IRequest<OperationResult<List<GetCollectedRewardByDatesModel>>>;

internal class GetCollectedRewardsByDatesQueryHandler(IDbContextFactory<StatisticsDbContext> dbContextFactory) : IRequestHandler<GetCollectedRewardsByDatesQuery, OperationResult<List<GetCollectedRewardByDatesModel>>>
{
    readonly IDbContextFactory<StatisticsDbContext> dbContextFactory = dbContextFactory;

    public async Task<OperationResult<List<GetCollectedRewardByDatesModel>>> Handle(GetCollectedRewardsByDatesQuery request, CancellationToken cancellationToken)
    {
        var (fromDateUtc, toDateUtc, errorMessage) = request.ValidateAndParseDates();

        if (errorMessage != null)
            return new OperationResult<List<GetCollectedRewardByDatesModel>>()
                .ReturnWithBadRequestException(errorMessage);

        using (var context = await dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            var rewards = await context.RewardHistoryLogs
                .Where(x =>
                    x.IsCollected &&
                    x.CollectedDate != null &&
                    x.CollectedDate.Value.Date >= fromDateUtc!.Value.Date &&
                    x.CollectedDate.Value.Date <= toDateUtc!.Value.Date)
                .GroupBy(x => x.RewardId)
                .Select(x => new GetCollectedRewardByDatesModel
                {
                    RewardId = x.Key,
                    CollectedCount = x.Count(),
                })
                .ToListAsync(cancellationToken);

            return new OperationResult<List<GetCollectedRewardByDatesModel>>(rewards);
        }
    }
}
