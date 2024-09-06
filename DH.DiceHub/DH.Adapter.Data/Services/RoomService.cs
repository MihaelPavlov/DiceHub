using DH.Domain.Models.RoomModels.Queries;
using DH.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Data.Services;

public class RoomService : IRoomService
{
    readonly IDbContextFactory<TenantDbContext> _contextFactory;

    public RoomService(IDbContextFactory<TenantDbContext> _contextFactory)
    {
        this._contextFactory = _contextFactory;
    }

    public async Task<GetRoomByIdQueryModel?> GetById(int id, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            return await (
                from r in context.Rooms
                join g in context.Games on r.GameId equals g.Id
                join gi in context.GameImages on g.Id equals gi.GameId
                where r.Id == id
                select new GetRoomByIdQueryModel
                {
                    Id = r.Id,
                    CreatedBy = r.UserId,
                    Name = r.Name,
                    StartDate = r.StartDate,
                    MaxParticipants = r.MaxParticipants,
                    GameId = r.GameId,
                    GameImageId = gi.Id,
                    JoinedParticipants = r.Participants.Count,
                }).FirstOrDefaultAsync(cancellationToken);
        }
    }

    public async Task<List<GetRoomListQueryModel>> GetListBySearchExpressionAsync(string searchExpression, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            return await (
                from r in context.Rooms
                join g in context.Games on r.GameId equals g.Id
                join gi in context.GameImages on g.Id equals gi.GameId
                where r.Name.ToLower().Contains(searchExpression.ToLower())
                select new GetRoomListQueryModel
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    Name = r.Name,
                    StartDate = r.StartDate,
                    MaxParticipants = r.MaxParticipants,
                    GameId = r.GameId,
                    GameImageId = gi.Id,
                    GameName = g.Name,
                    JoinedParticipants = r.Participants.Count,
                }).ToListAsync(cancellationToken);
        }
    }
}
