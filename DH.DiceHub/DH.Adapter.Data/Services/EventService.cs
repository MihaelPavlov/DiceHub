using DH.Domain.Entities;
using DH.Domain.Services;

namespace DH.Adapter.Data.Services;

internal class EventService : IEventService
{
    public Task<int> CreateEvent(Event eventModel, string? fileName, string? contentType, MemoryStream? imageStream, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
