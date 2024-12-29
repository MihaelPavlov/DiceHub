using DH.OperationResultCore.Extension;
using DH.OperationResultCore.Utility;
using DH.Statistics.Data;
using DH.Statistics.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DH.Statistics.Application.Commands;

public record RemoveEventAttendanceLogCommand(RemoveEventAttendanceLogRequest Log) : IRequest<OperationResult<int>>;

internal class RemoveEventAttendanceLogCommandHandler(IDbContextFactory<StatisticsDbContext> dbContext) : IRequestHandler<RemoveEventAttendanceLogCommand, OperationResult<int>>
{
    readonly IDbContextFactory<StatisticsDbContext> dbContext = dbContext;

    public async Task<OperationResult<int>> Handle(RemoveEventAttendanceLogCommand request, CancellationToken cancellationToken)
    {
        using (var context = await dbContext.CreateDbContextAsync(cancellationToken))
        {
            var log = await context.EventAttendanceLogs
                .FirstOrDefaultAsync(x =>
                    x.UserId == request.Log.UserId &&
                    x.EventId == request.Log.EventId
                );

            if (log == null)
                return new OperationResult<int>(request.Log.EventId)
                    .ReturnWithNotFoundException(nameof(EventAttendanceLog), request.Log.EventId);

            context.EventAttendanceLogs.Remove(log);

            await context.SaveChangesAsync(cancellationToken);

            return new OperationResult<int>(log.Id);
        }
    }
}

public record RemoveEventAttendanceLogRequest(string UserId, int EventId);