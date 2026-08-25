using DH.Domain.Adapters.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using System.Security;

namespace DH.Adapter.Authentication;

/// <summary>
/// Entity Framework Core connection interceptor that applies
/// tenant-specific context to database connections.
/// </summary>
/// <remarks>
/// This interceptor sets the PostgreSQL session variable <c>app.tenant_id</c>
/// based on the current HTTP request context.
/// It is intended to support Row-Level Security (RLS) and tenant isolation.
/// </remarks>
public class ApplicationDbConnectionInterceptor : DbConnectionInterceptor
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ISystemUserContextAccessor systemUserContextAccessor;

    public ApplicationDbConnectionInterceptor(
        IHttpContextAccessor httpContextAccessor,
        ISystemUserContextAccessor systemUserContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.systemUserContextAccessor = systemUserContextAccessor;
    }

    public override async Task ConnectionOpenedAsync(
        DbConnection connection,
        ConnectionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        var httpContext = httpContextAccessor.HttpContext;

        // No HttpContext at all (a genuine background job/worker not wrapped in
        // ITenantContextScopeRunner) and no scoped system tenant either: preserve the
        // original no-op behavior rather than start throwing for callers that never
        // needed a tenant here.
        if (httpContext == null && string.IsNullOrEmpty(systemUserContextAccessor.Peek.TenantId))
            return;

        if (httpContext != null
            && httpContext.Request.Headers.TryGetValue("X-Requires-Tenant", out var value)
            && value == "false"
            && httpContext.Items["TenantId"]?.ToString() == null)
        {
            return;
        }

        var tenantId = ResolveTenantId(httpContext);
        if (string.IsNullOrEmpty(tenantId))
            throw new SecurityException("Tenant context missing");

        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = $"SET app.tenant_id = '{tenantId.Replace("'", "''")}'";
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }

        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
    }

    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        var httpContext = httpContextAccessor.HttpContext;

        if (httpContext == null && string.IsNullOrEmpty(systemUserContextAccessor.Peek.TenantId))
            return;

        if (httpContext != null
            && httpContext.Request.Headers.TryGetValue("X-Requires-Tenant", out var value)
            && value == "false"
            && httpContext.Items["TenantId"]?.ToString() == null)
        {
            return;
        }

        var tenantId = ResolveTenantId(httpContext);
        if (string.IsNullOrEmpty(tenantId))
            throw new SecurityException("Tenant context missing");

        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = $"SET app.tenant_id = '{tenantId.Replace("'", "''")}'";
            cmd.ExecuteNonQuery();
        }

        base.ConnectionOpened(connection, eventData);
    }

    /// <summary>
    /// Route/header wins (set by TenantRouteValidationMiddleware on the original HTTP
    /// request). Falls back to the "tenant_id" JWT claim directly, because that
    /// middleware only runs once per HTTP request - it never sees the later invocations
    /// of a long-lived SignalR connection (e.g. a SignalR hub method call), whose
    /// HttpContext.User is instead populated after the fact. Falls back last to the
    /// scoped background-job system context (Quartz jobs, hosted workers,
    /// ITenantContextScopeRunner).
    /// </summary>
    private string? ResolveTenantId(HttpContext? httpContext)
    {
        return httpContext?.Items["TenantId"]?.ToString()
            ?? httpContext?.User?.FindFirst("tenant_id")?.Value
            ?? systemUserContextAccessor.Peek.TenantId;
    }
}
