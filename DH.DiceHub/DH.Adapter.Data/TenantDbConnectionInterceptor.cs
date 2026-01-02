using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;
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

    public TenantDbConnectionInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public override async Task ConnectionOpenedAsync(
        DbConnection connection,
        ConnectionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        var httpContext = httpContextAccessor.HttpContext;

        if (httpContext == null)
            return;

        var tenantId = httpContext.Items["TenantId"]?.ToString();
        if (httpContext.Request.Headers.TryGetValue("X-Requires-Tenant", out var value)
            && value == "false" && tenantId == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(tenantId))
            throw new SecurityException("Tenant context missing");

        if (!string.IsNullOrEmpty(tenantId))
        {
            using (var cmd = connection.CreateCommand())
            {

                cmd.CommandText = $"SET app.tenant_id = '{tenantId.Replace("'", "''")}'";
                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }
        }

        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
    }

    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
     {
        if (this.httpContextAccessor != null && this.httpContextAccessor.HttpContext != null)
        {
            var tenantId = this.httpContextAccessor.HttpContext?
                .Items["TenantId"]?.ToString();

            if (!string.IsNullOrEmpty(tenantId))
            {
                using (var cmd = connection.CreateCommand())
                {

                    cmd.CommandText = $"SET app.tenant_id = '{tenantId.Replace("'", "''")}'";
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
