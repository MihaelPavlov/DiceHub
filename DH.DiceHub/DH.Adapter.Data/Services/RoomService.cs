using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Entities;
using DH.Domain.Models.RoomModels.Queries;
using DH.Domain.Services;
using DH.OperationResultCore.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Data.Services;

public class RoomService : IRoomService
{
    readonly IDbContextFactory<TenantDbContext> _contextFactory;
    readonly IUserContext userContext;
    readonly IPushNotificationsService pushNotificationsService;

    public RoomService(
        IDbContextFactory<TenantDbContext> _contextFactory,
        IUserContext userContext, IPushNotificationsService pushNotificationsService)
    {
        this._contextFactory = _contextFactory;
        this.userContext = userContext;
        this.pushNotificationsService = pushNotificationsService;
    }

    public async Task Delete(int id, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            using (var transaction = await context.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    var room = await context.Rooms.FirstOrDefaultAsync(g => g.Id == id, cancellationToken)
                        ?? throw new NotFoundException(nameof(Room), id);

                    if (room.UserId != this.userContext.UserId)
                        throw new BadRequestException("Cannot delete room if you are not the creator of it");

                    var roomParticipants = await context.RoomParticipants
                        .Where(x => x.RoomId == id)
                        .ToListAsync(cancellationToken);

                    var roomMessages = await context.RoomMessages
                        .Where(x => x.RoomId == id)
                        .ToListAsync(cancellationToken);

                    context.RoomParticipants.RemoveRange(roomParticipants);
                    context.RoomMessages.RemoveRange(roomMessages);
                    context.Rooms.Remove(room);

                    await context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            }
        }
    }

    public async Task<GetRoomByIdQueryModel?> GetById(int id, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            return await (
                from r in context.Rooms.AsNoTracking()
                where r.Id == id
                select new GetRoomByIdQueryModel
                {
                    Id = r.Id,
                    CreatedBy = r.UserId,
                    Name = r.Name,
                    StartDate = r.StartDate,
                    MaxParticipants = r.MaxParticipants,
                    GameId = r.GameId,
                    GameImageUrl = r.Game.ImageUrl,
                    JoinedParticipants = r.Participants.Where(x => !x.IsDeleted).Count(),
                }).FirstOrDefaultAsync(cancellationToken);
        }
    }

    public async Task<List<GetRoomListQueryModel>> GetListBySearchExpressionAsync(string searchExpression, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var today = DateTime.UtcNow;
            return await (
                from r in context.Rooms.AsNoTracking()
                where r.Name.ToLower().Contains(searchExpression.ToLower()) && r.StartDate > today
                select new GetRoomListQueryModel
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    Name = r.Name,
                    StartDate = r.StartDate,
                    MaxParticipants = r.MaxParticipants,
                    GameId = r.GameId,
                    GameImageUrl = r.Game.ImageUrl,
                    GameName = r.Game.Name,
                    JoinedParticipants = r.Participants.Where(x => !x.IsDeleted).Count(),
                })
                .OrderBy(x => x.StartDate)
                .ToListAsync(cancellationToken);
        }
    }

    public async Task Update(Room updatedRoom, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            using (var transaction = await context.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    var game = await context.Games.FirstOrDefaultAsync(x => x.Id == updatedRoom.GameId, cancellationToken)
                        ?? throw new NotFoundException(nameof(Game), updatedRoom.GameId);

                    var room = await context.Rooms
                        .Include(x => x.Participants)
                        .Include(x => x.Game)
                        .AsTracking()
                        .FirstOrDefaultAsync(g => g.Id == updatedRoom.Id, cancellationToken)
                            ?? throw new NotFoundException(nameof(Room), updatedRoom.Id);

                    if (room.UserId != this.userContext.UserId)
                        throw new BadRequestException("Cannot delete room if you are not the creator of it");

                    var userIdsForNotification = room.Participants.Select(x => x.UserId).ToList();

                    if (room.GameId != updatedRoom.GameId)
                    {
                        await context.AddAsync(new RoomInfoMessage
                        {
                            CreatedBy = this.userContext.UserId,
                            CreatedDate = DateTime.UtcNow,
                            MessageContentKey = "RoomGameChanged",
                            RoomId = room.Id,
                        }, cancellationToken);

                        var payload = new RoomGameChangedNotification
                        {
                            NewGameName = game.Name,
                            OldGameName = room.Game.Name,
                            RoomName = room.Name
                        };

                        await this.pushNotificationsService
                            .SendNotificationToUsersAsync(userIdsForNotification, payload, cancellationToken);

                    }
                    if (room.StartDate != updatedRoom.StartDate)
                    {
                        await context.AddAsync(new RoomInfoMessage
                        {
                            CreatedBy = this.userContext.UserId,
                            CreatedDate = DateTime.UtcNow,
                            MessageContentKey = "RoomStartDateChanged",
                            RoomId = room.Id,
                        }, cancellationToken);


                        var payload = new RoomStartDateChangedNotification
                        {
                            NewDate = updatedRoom.StartDate,
                            OldDate = room.StartDate,
                            RoomName = room.Name
                        }; ;

                        await this.pushNotificationsService
                            .SendNotificationToUsersAsync(userIdsForNotification, payload, cancellationToken);
                    }

                    room.StartDate = updatedRoom.StartDate;
                    room.GameId = updatedRoom.GameId;
                    room.Name = updatedRoom.Name;
                    room.MaxParticipants = updatedRoom.MaxParticipants;

                    await context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            }
        }
    }
}
