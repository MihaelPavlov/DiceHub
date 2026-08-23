using DH.Domain.Adapters.Authentication;
using DH.Domain.Services;

namespace DH.Adapter.Authentication.Helper;

public class TenantContextScopeRunner : ITenantContextScopeRunner
{
    readonly ISystemUserContextAccessor systemUserContextAccessor;

    public TenantContextScopeRunner(ISystemUserContextAccessor systemUserContextAccessor)
    {
        this.systemUserContextAccessor = systemUserContextAccessor;
    }

    public async Task RunAsTenantAsync(string tenantId, Func<Task> action)
    {
        this.systemUserContextAccessor.Set(new BackgroundJobUserContext(tenantId));
        try
        {
            await action();
        }
        finally
        {
            this.systemUserContextAccessor.Set(AnonymousUserContext.Instance);
        }
    }

    public async Task<T> RunAsTenantAsync<T>(string tenantId, Func<Task<T>> action)
    {
        this.systemUserContextAccessor.Set(new BackgroundJobUserContext(tenantId));
        try
        {
            return await action();
        }
        finally
        {
            this.systemUserContextAccessor.Set(AnonymousUserContext.Instance);
        }
    }
}
