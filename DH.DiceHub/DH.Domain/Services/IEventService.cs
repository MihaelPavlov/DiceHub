using DH.Domain.Entities;

namespace DH.Domain.Services;

public interface IEventService : IDomainService<Event>
{
    Task<int> CreateEvent(Event eventModel, string? fileName, string? contentType, MemoryStream? imageStream, CancellationToken cancellationToken);
}
