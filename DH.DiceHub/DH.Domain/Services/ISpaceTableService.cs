using DH.Domain.Entities;

namespace DH.Domain.Services;

public interface ISpaceTableService : IDomainService<SpaceTable>
{
    Task<int> Create(SpaceTable spaceTable,CancellationToken cancellationToken);
}
