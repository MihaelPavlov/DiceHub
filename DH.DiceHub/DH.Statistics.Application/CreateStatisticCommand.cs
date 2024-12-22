using DH.Statistics.Data;
using DH.Statistics.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DH.Statistics.Application;

public record CreateStatisticCommand : IRequest;

internal class CreateStatisticCommandHandler(IDbContextFactory<StatisticsDbContext> dbContext) : IRequestHandler<CreateStatisticCommand>
{
    readonly IDbContextFactory<StatisticsDbContext> dbContext = dbContext;

    public async Task Handle(CreateStatisticCommand request, CancellationToken cancellationToken)
    {
        using (var context = dbContext.CreateDbContext())
        {
            await context.Games.AddAsync(new Game
            {
                Name = "Game 1",
            }, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
