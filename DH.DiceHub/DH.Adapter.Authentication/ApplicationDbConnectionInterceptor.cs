using DH.Domain.Adapters.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

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
    private readonly ITenantExecutionContextAccessor tenantExecutionContextAccessor;

    public ApplicationDbConnectionInterceptor(
        IHttpContextAccessor httpContextAccessor,
        ITenantExecutionContextAccessor tenantExecutionContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.tenantExecutionContextAccessor = tenantExecutionContextAccessor;
    }

    public override async Task ConnectionOpenedAsync(
        DbConnection connection,
        ConnectionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        var httpContext = this.httpContextAccessor.HttpContext;

        if (httpContext == null)
            return;

        var tenantId = httpContext.Items["TenantId"]?.ToString()
            ?? this.tenantExecutionContextAccessor.TenantId;

        if (httpContext.Request.Headers.TryGetValue("X-Requires-Tenant", out var value)
            && value == "false" && tenantId == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(tenantId))
            return;

        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = $"SET app.tenant_id = '{tenantId.Replace("'", "''")}'";
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }

        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
    }

    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        if (this.httpContextAccessor.HttpContext == null)
            return;

        var tenantId = this.httpContextAccessor.HttpContext.Items["TenantId"]?.ToString()
            ?? this.tenantExecutionContextAccessor.TenantId;

        if (string.IsNullOrEmpty(tenantId))
            return;

        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = $"SET app.tenant_id = '{tenantId.Replace("'", "''")}'";
            cmd.ExecuteNonQuery();
        }
    }
}
