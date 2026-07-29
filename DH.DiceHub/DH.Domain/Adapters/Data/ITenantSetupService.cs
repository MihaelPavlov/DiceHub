using DH.Domain.Models.Common;

namespace DH.Domain.Adapters.Data;

public interface ITenantSetupService
{
    Task<CompleteTenantSetupResult> CompleteTenantSetupData(
        CompleteTenantSetupRequest request,
        string tokenHash,
        CancellationToken cancellationToken);

    Task MarkSetupTokenAsUsed(string tokenHash, CancellationToken cancellationToken);
}
