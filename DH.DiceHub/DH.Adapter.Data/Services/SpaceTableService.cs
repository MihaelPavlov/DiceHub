﻿using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Services;
using DH.OperationResultCore.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Data.Services;

public class SpaceTableService : ISpaceTableService
{
    readonly IUserContext userContext;
    readonly IDbContextFactory<TenantDbContext> dbContextFactory;

    public SpaceTableService(IUserContext userContext, IDbContextFactory<TenantDbContext> dbContextFactory)
    {
        this.userContext = userContext;
        this.dbContextFactory = dbContextFactory;
    }

    public async Task<int> Create(SpaceTable spaceTable, CancellationToken cancellationToken, bool fromGameReservation = false, string userId = "")
    {
        using (var context = await this.dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            using (var transaction = await context.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    var currentUser = fromGameReservation ? userId : this.userContext.UserId;
                    var doesUserHaveActiveTable = await context.SpaceTables
                   .AnyAsync(x =>
                       x.IsTableActive &&
                       x.CreatedBy == currentUser, cancellationToken);

                    var userParticipateInAnyActiveTables = await context.SpaceTableParticipants
                        .Where(x =>
                            x.UserId == currentUser &&
                            x.SpaceTable.IsTableActive
                        ).ToListAsync(cancellationToken);

                    if (doesUserHaveActiveTable)
                        throw new ValidationErrorsException("UserHaveActiveTable", "You already have an active table");

                    if (userParticipateInAnyActiveTables.Count != 0)
                        throw new ValidationErrorsException("UserParticipateInActiveTable", "You already participate in table");

                    if (!fromGameReservation)
                    {
                        var gameInvetory = await context.GameInventories.AsTracking()
                            .FirstOrDefaultAsync(x => x.GameId == spaceTable.GameId, cancellationToken);

                        if (gameInvetory == null || gameInvetory.AvailableCopies <= 0)
                            throw new ValidationErrorsException("NoAvailableCopies", "No Available copies of this game.");

                        gameInvetory.AvailableCopies--;
                    }

                    spaceTable.IsTableActive = true;
                    spaceTable.IsLocked = !string.IsNullOrEmpty(spaceTable.Password);
                    spaceTable.CreatedBy = currentUser;
                    spaceTable.CreatedDate = DateTime.UtcNow;

                    var createdSpaceTable = await context.SpaceTables.AddAsync(spaceTable, cancellationToken);

                    await context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    return createdSpaceTable.Entity.Id;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            }
        }
    }
}
