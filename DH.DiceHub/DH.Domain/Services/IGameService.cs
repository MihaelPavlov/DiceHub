using DH.Domain.Entities;
using DH.Domain.Models.GameModels;

namespace DH.Domain.Services;

public interface IGameService : IDomainService<Game>
{
    Task<List<GameComplexDataQuery>> GetCompexDataAsync( CancellationToken cancellationToken);
}
