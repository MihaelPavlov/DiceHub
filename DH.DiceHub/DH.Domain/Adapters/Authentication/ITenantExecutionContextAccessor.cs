namespace DH.Domain.Adapters.Authentication;

public interface ITenantExecutionContextAccessor
{
    string? TenantId { get; set; }
    void Clear();
}
