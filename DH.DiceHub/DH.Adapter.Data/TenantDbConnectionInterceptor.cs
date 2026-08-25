using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;
using DH.Domain.Adapters.Authentication;
using System.Data.Common;
using System.Security;

namespace DH.Adapter.Data;

/// <summary>
/// Entity Framework Core connection interceptor that applies
/// tenant-specific context to database connections.
/// </summary>
/// <remarks>
/// This interceptor sets the PostgreSQL session variable <c>app.tenant_id</c>
/// based on the current HTTP request context.
/// It is intended to support Row-Level Security (RLS) and tenant isolation.
/// </remarks>
public class TenantDbConnectionInterceptor : DbConnectionInterceptor
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ISystemUserContextAccessor systemUserContextAccessor;

    public TenantDbConnectionInterceptor(
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
        using (var resetCommand = connection.CreateCommand())
        {
            // Pooled connections must not retain the previous request's tenant.
            resetCommand.CommandText = "RESET app.tenant_id";
            await resetCommand.ExecuteNonQueryAsync(cancellationToken);
        }

        var tenantId = ResolveTenantId();
        if (!string.IsNullOrEmpty(tenantId))
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"SET app.tenant_id = '{tenantId.Replace("'", "''")}'";
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
    }

    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
     {
        using (var resetCommand = connection.CreateCommand())
        {
            resetCommand.CommandText = "RESET app.tenant_id";
            resetCommand.ExecuteNonQuery();
        }

        var tenantId = ResolveTenantId();
        if (!string.IsNullOrEmpty(tenantId))
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SET app.tenant_id = '{tenantId.Replace("'", "''")}'";
                command.ExecuteNonQuery();
            }
        }

        base.ConnectionOpened(connection, eventData);
    }

    /// <summary>
    /// Resolves the tenant to apply to this connection's RLS session variable.
    /// Route/header wins (set by TenantRouteValidationMiddleware on the original
    /// HTTP request). Falls back to the "tenant_id" JWT claim directly, because
    /// that middleware only runs once per HTTP request - it never sees the later
    /// invocations of a long-lived SignalR connection, whose HttpContext.User is
    /// instead populated after the fact (e.g. ChatHubClient.OnConnectedAsync
    /// validating the access_token query param). Falls back last to the
    /// background-job system context (Quartz jobs, hosted workers) which has
    /// no HttpContext at all.
    /// </summary>
    private string? ResolveTenantId()
    {
        var httpContext = httpContextAccessor.HttpContext;

        return httpContext?.Items["TenantId"]?.ToString()
            ?? httpContext?.User?.FindFirst("tenant_id")?.Value
            ?? systemUserContextAccessor.Peek.TenantId;
    }
}
