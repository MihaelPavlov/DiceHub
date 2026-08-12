using DH.Domain.Adapters.Authentication;
using System.Threading;

namespace DH.Adapter.Authentication.Helper;

public class TenantExecutionContextAccessor : ITenantExecutionContextAccessor
{
    private static readonly AsyncLocal<string?> CurrentTenant = new();

    public string? TenantId
    {
        get => CurrentTenant.Value;
        set => CurrentTenant.Value = value;
    }

    public void Clear()
    {
        CurrentTenant.Value = null;
    }
}
