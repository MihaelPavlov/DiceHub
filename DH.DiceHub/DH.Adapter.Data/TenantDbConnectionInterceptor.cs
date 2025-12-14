using DH.Domain.Adapters.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using System.Threading;

namespace DH.Adapter.Data;

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
        var tenantId = this.httpContextAccessor.HttpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();

        if (!string.IsNullOrEmpty(tenantId))
        {
            using (var cmd = connection.CreateCommand())
            {

                cmd.CommandText = $"SET app.tenant_id = '{tenantId.Replace("'", "''")}'";
                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }
            try
            {

                //await cmd.ExecuteNonQueryAsync(cancellationToken);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
    }

    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        if (this.httpContextAccessor != null && this.httpContextAccessor.HttpContext != null)
        {
        var tenantId = this.httpContextAccessor.HttpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();

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
