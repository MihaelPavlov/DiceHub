using DH.Messaging.HttpClient.UserContext;
using DH.OperationResultCore.Utility;
using DH.Statistics.Data;
using DH.Statistics.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DH.Statistics.Application.Commands;

public record CreateRewardHistoryLogCommand(CreateRewardHistoryLogRequest Log) : IRequest<OperationResult<int>>;

internal class CreateRewardHistoryLogCommandHandler(IDbContextFactory<StatisticsDbContext> dbContext,IUserContext userContext) : IRequestHandler<CreateRewardHistoryLogCommand, OperationResult<int>>
{
    readonly IDbContextFactory<StatisticsDbContext> dbContext = dbContext;
    readonly IUserContext userContext = userContext;

    public async Task<OperationResult<int>> Handle(CreateRewardHistoryLogCommand request, CancellationToken cancellationToken)
    {
        using (var context = await dbContext.CreateDbContextAsync(cancellationToken))
        {
            var result = await context.RewardHistoryLogs.AddAsync(new RewardHistoryLog
            {
                UserId = request.Log.UserId,
                RewardId = request.Log.RewardId,
                CollectedDate = request.Log.CollectedDate,
                ExpiredDate = request.Log.ExpiredDate,
                IsCollected = request.Log.IsCollected ?? false,
                IsExpired = request.Log.IsExpired ?? false,
                CreatedDate = DateTime.UtcNow,
            }, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            return new OperationResult<int>(result.Entity.Id);
        }
    }
}

public record CreateRewardHistoryLogRequest(string UserId, int RewardId, bool? IsCollected, bool? IsExpired, DateTime CollectedDate, DateTime ExpiredDate);
