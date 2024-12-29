using DH.OperationResultCore.Utility;
using DH.Statistics.Data;
using DH.Statistics.Domain.Entities;
using DH.Statistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DH.Statistics.Application.Commands;

public record CreateReservationOutcomeCommand(CreateReservationOutcomeRequest Log) : IRequest<OperationResult<int>>;

internal class CreateReservationOutcomeCommandHandler(IDbContextFactory<StatisticsDbContext> dbContext) : IRequestHandler<CreateReservationOutcomeCommand, OperationResult<int>>
{
    readonly IDbContextFactory<StatisticsDbContext> dbContext = dbContext;

    public async Task<OperationResult<int>> Handle(CreateReservationOutcomeCommand request, CancellationToken cancellationToken)
    {
        using (var context = await dbContext.CreateDbContextAsync(cancellationToken))
        {
            var result = await context.ReservationOutcomeLogs.AddAsync(new ReservationOutcomeLog
            {
                ReservationId = request.Log.ReservationId,
                Type = request.Log.Type,
                UserId = request.Log.UserId,
                Outcome = request.Log.Outcome,
                OutcomeDate = request.Log.OutcomeDate,
                CreatedDate = DateTime.UtcNow,
            }, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            return new OperationResult<int>(result.Entity.Id);
        }
    }
}

public record CreateReservationOutcomeRequest(string UserId, int ReservationId, ReservationType Type, DateTime OutcomeDate, ReservationOutcome Outcome);
