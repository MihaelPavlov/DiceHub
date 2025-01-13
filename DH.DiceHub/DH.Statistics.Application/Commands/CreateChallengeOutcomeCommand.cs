using DH.Messaging.HttpClient.UserContext;
using DH.OperationResultCore.Extension;
using DH.OperationResultCore.Utility;
using DH.Statistics.Data;
using DH.Statistics.Domain.Entities;
using DH.Statistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DH.Statistics.Application.Commands;

public record CreateChallengeOutcomeCommand(CreateChallengeOutcomeRequest Log) : IRequest<OperationResult<int>>;

internal class CreateChallengeOutcomeCommandHandler(IDbContextFactory<StatisticsDbContext> dbContext) : IRequestHandler<CreateChallengeOutcomeCommand, OperationResult<int>>
{
    readonly IDbContextFactory<StatisticsDbContext> dbContext = dbContext;

    public async Task<OperationResult<int>> Handle(CreateChallengeOutcomeCommand request, CancellationToken cancellationToken)
    {
        var isParsed = Enum.TryParse<ChallengeOutcome>(request.Log.Outcome.ToString(), out var parsedOutcome);
        if (isParsed)
        {
            var operationResult = new OperationResult<int>();
            operationResult.AppendValidationError("Outcome was not parsed correct", nameof(request.Log.Outcome));
            return operationResult.ReturnWithValidationException(operationResult.ValidationErrors);
        }

        using (var context = await dbContext.CreateDbContextAsync(cancellationToken))
        {
            var result = await context.ChallengeHistoryLogs.AddAsync(new ChallengeHistoryLog
            {
                UserId = request.Log.UserId,
                ChallengeId = request.Log.ChallengeId,
                OutcomeDate = request.Log.OutcomeDate,
                Outcome = parsedOutcome,
                CreatedDate = DateTime.UtcNow,
            }, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            return new OperationResult<int>(result.Entity.Id);
        }
    }
}

public record CreateChallengeOutcomeRequest(int ChallengeId, string UserId, DateTime OutcomeDate, int Outcome);