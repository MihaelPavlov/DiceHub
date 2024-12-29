using DH.OperationResultCore.Utility;
using DH.Statistics.Data;
using DH.Statistics.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DH.Statistics.Application.Commands;

public record CreateEventAttendanceLogCommand(CreateEventAttendanceLogRequest Log) : IRequest<OperationResult<int>>;

internal class CreateEventAttendanceLogCommandHandler(IDbContextFactory<StatisticsDbContext> dbContext) : IRequestHandler<CreateEventAttendanceLogCommand, OperationResult<int>>
{
    readonly IDbContextFactory<StatisticsDbContext> dbContext = dbContext;

    public async Task<OperationResult<int>> Handle(CreateEventAttendanceLogCommand request, CancellationToken cancellationToken)
    {
        using (var context = await dbContext.CreateDbContextAsync(cancellationToken))
        {
            var result = await context.EventAttendanceLogs.AddAsync(new EventAttendanceLog
            {
                UserId = request.Log.UserId,
                LogDate = request.Log.LogDate,
                CreatedDate = DateTime.UtcNow,
                EventId = request.Log.EventId
            }, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            return new OperationResult<int>(result.Entity.Id);
        }
    }
}

public record CreateEventAttendanceLogRequest(string UserId, int EventId, DateTime LogDate);