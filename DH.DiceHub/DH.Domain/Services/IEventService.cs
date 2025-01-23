using DH.Domain.Entities;
using DH.Domain.Models.EventModels.Queries;

namespace DH.Domain.Services;

public interface IEventService : IDomainService<Event>
{
    Task<int> CreateEvent(Event eventModel, string? fileName, string? contentType, MemoryStream? imageStream, CancellationToken cancellationToken);
    Task UpdateEvent(Event eventModel, string? fileName, string? contentType, MemoryStream? imageStream, CancellationToken cancellationToken);
    Task<List<GetEventListQueryModel>> GetListForUsers(CancellationToken cancellationToken);
    Task<List<GetEventListQueryModel>> GetListForStaff(string searchExpression, CancellationToken cancellationToken);
    Task<List<GetEventListQueryModel>> GetUserEvents(CancellationToken cancellationToken);
    Task<GetEventByIdQueryModel?> GetById(int eventId, CancellationToken cancellationToken);
}
