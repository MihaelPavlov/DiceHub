using DH.Domain.Adapters.Data;
using System.Security.Claims;

namespace DH.Api;

/// <summary>
/// Middleware responsible for validating tenant access based on route and JWT claims.
/// </summary>
/// <remarks>
/// This middleware ensures that:
/// <list type="bullet">
/// <item>The tenant slug in the route exists.</item>
/// <item>The authenticated user contains a tenant claim.</item>
/// <item>The tenant in the JWT matches the tenant in the route.</item>
/// </list>
/// When validation succeeds, the resolved tenant ID is exposed via
/// <see cref="HttpContext.Items"/> for downstream components.
/// </remarks>
public class TenantRouteValidationMiddleware
{
    private readonly RequestDelegate _next;

    public TenantRouteValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ITenantService tenantResolver)
    {
        // Extract tenant from route (e.g., /api/{tenant}/...)
        var routeTenant = context.Request.RouteValues["tenant"]?.ToString();

        // If no tenant in route, skip validation
        if (string.IsNullOrWhiteSpace(routeTenant))
        {
            await _next(context);
            return;
        }

        // Allow anonymous endpoints if JWT is missing
        var user = context.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Authentication required.");
            return;
        }

        // Pre-seed TenantId so the DB interceptor has a non-empty value during
        // the tenant-resolution query. The Tenants table has no per-tenant RLS,
        // so any non-empty candidate value is safe. It is overwritten below with
        // the authoritative DB id once the lookup succeeds.
        context.Items["TenantId"] = routeTenant;

        // Resolve tenant by slug from route
        var tenant = await tenantResolver.GetByRouteIdentifier(routeTenant);
        if (tenant == null)
        {
            context.Items.Remove("TenantId");
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsync("Tenant not found.");
            return;
        }

        // Extract tenantId from JWT claim
        var tokenTenantId = user.FindFirstValue("tenant_id");
        var isSuperAdmin = user.IsInRole("SuperAdmin");

        // Super admins may access any tenant route regardless of their token's tenant claim.
        // Regular users must match the route tenant exactly.
        if (!isSuperAdmin && tenant.Id != tokenTenantId)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Tenant mismatch.");
            return;
        }

        // Expose tenant for DbContext / RLS
        context.Items["TenantId"] = tenant.Id;

        // Continue pipeline
        await _next(context);
    }
}
