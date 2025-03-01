﻿using DH.OperationResultCore.Utility;
using DH.Statistics.Data;
using DH.Statistics.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DH.Statistics.Application.Commands;

public record CreateClubVisitorLogCommand(CreateClubVisitorLogRequest Log) : IRequest<OperationResult<int>>;

internal class CreateClubVisitorLogCommandHandler(IDbContextFactory<StatisticsDbContext> dbContext) : IRequestHandler<CreateClubVisitorLogCommand, OperationResult<int>>
{
    readonly IDbContextFactory<StatisticsDbContext> dbContext = dbContext;

    public async Task<OperationResult<int>> Handle(CreateClubVisitorLogCommand request, CancellationToken cancellationToken)
    {
        using (var context = await dbContext.CreateDbContextAsync(cancellationToken))
        {
            var result = await context.ClubVisitorLogs.AddAsync(new ClubVisitorLog
            {
                UserId = request.Log.UserId,
                LogDate = request.Log.LogDate,
                CreatedDate = DateTime.UtcNow,
            }, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            return new OperationResult<int>(result.Entity.Id);
        }
    }
}

public record CreateClubVisitorLogRequest(string UserId, DateTime LogDate);