using DH.Domain.Entities;
using DH.Domain.Models.RoomModels.Queries;

namespace DH.Domain.Services;

public interface IRoomService : IDomainService<Room>
{
    Task<List<GetRoomListQueryModel>> GetListBySearchExpressionAsync(string searchExpression, CancellationToken cancellationToken);
    Task<GetRoomByIdQueryModel?> GetById(int id, CancellationToken cancellationToken);
}
