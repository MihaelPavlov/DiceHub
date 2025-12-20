using DH.Domain.Adapters.Authentication.Models;

namespace DH.Domain.Adapters.Authentication.Services;

public interface IOwnerService
{
    Task<OwnerResult> CreateOwner(CreateOwnerRequest request, CancellationToken cancellationToken);
    Task CreateOwnerPassword(CreateOwnerPasswordRequest request);
    Task<OwnerResult?> GetOwner(CancellationToken cancellationToken);
    Task DeleteOwner(CancellationToken cancellationToken);
}
