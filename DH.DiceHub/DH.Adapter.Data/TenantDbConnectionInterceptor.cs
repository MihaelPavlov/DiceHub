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

        var tenantId = httpContextAccessor.HttpContext?.Items["TenantId"]?.ToString()
            ?? systemUserContextAccessor.Peek.TenantId;
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

        var tenantId = httpContextAccessor.HttpContext?.Items["TenantId"]?.ToString()
            ?? systemUserContextAccessor.Peek.TenantId;
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
}
