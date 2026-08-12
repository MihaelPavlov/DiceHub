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

        var user = context.User;
        var tokenTenantId = user?.FindFirstValue("tenant_id");
        var headerTenantId = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        var effectiveTenant = routeTenant ?? headerTenantId;

        if (string.IsNullOrWhiteSpace(effectiveTenant)
            && user?.Identity?.IsAuthenticated == true
            && !string.Equals(tokenTenantId, "system", StringComparison.OrdinalIgnoreCase))
        {
            effectiveTenant = tokenTenantId;
        }

        // No tenant means this is an explicitly system/public request.
        if (string.IsNullOrWhiteSpace(effectiveTenant))
        {
            await _next(context);
            return;
        }

        var tenant = await tenantResolver.GetByTenantName(effectiveTenant);
        if (tenant == null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsync("Tenant not found.");
            return;
        }

        var isSuperAdmin = context.User.IsInRole("SuperAdmin");

        // Normal users can only access their token tenant. SuperAdmin may
        // select a validated tenant through the route/header.
        if (context.User.Identity?.IsAuthenticated == true
            && !isSuperAdmin
            && tenant.Id != tokenTenantId)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Tenant mismatch.");
            return;
        }

        // Expose tenant for DbContext / RLS
        if (!string.IsNullOrWhiteSpace(routeTenant)
            && !string.IsNullOrWhiteSpace(headerTenantId)
            && !string.Equals(routeTenant, headerTenantId, StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Tenant route and tenant header do not match.");
            return;
        }

        context.Items["TenantId"] = tenant.Id;

        // Continue pipeline
        await _next(context);
    }
}
