using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Enums;
using DH.Domain.Services.TenantUserSettingsService;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DH.Adapter.Authentication.Helper;

public class UserContextFactory : IUserContextFactory
{
    readonly IHttpContextAccessor httpContextAccessor;
    readonly ITokenService jwtService;
    readonly IUserSettingsCache userSettingsCache;

    public UserContextFactory(IHttpContextAccessor httpContextAccessor, ITokenService jwtService, IUserSettingsCache userSettingsCache)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.jwtService = jwtService;
        this.userSettingsCache = userSettingsCache;
    }

    /// <summary>
    /// Creates a user context from the current HTTP request.
    /// Uses ASP.NET authentication state ONLY.
    /// </summary>
    public async Task<IUserContext> CreateAsync()
    {
        var httpContext = this.httpContextAccessor.HttpContext;
        var user = httpContext?.User;
        // Resolved by TenantRouteValidationMiddleware from the route/X-Tenant-Id
        // header. Available even for anonymous requests (e.g. the pre-auth
        // login/register/forgot-password pages), so it must not be discarded
        // just because there's no authenticated user yet.
        var routeOrHeaderTenantId = httpContext?.Items["TenantId"]?.ToString();

        if (user?.Identity?.IsAuthenticated != true)
        {
            return string.IsNullOrWhiteSpace(routeOrHeaderTenantId)
                ? AnonymousUserContext.Instance
                : new UserContext(routeOrHeaderTenantId, null, null, null, null);
        }

        var userId = user.FindFirstValue(ClaimTypes.Sid);
        var roleName = user.FindFirstValue(ClaimTypes.Role);
        var timeZone = user.FindFirstValue("TimeZone");
        var tenantId = routeOrHeaderTenantId
            ?? user.FindFirstValue("tenant_id");

        if (string.IsNullOrWhiteSpace(userId))
        {
            return string.IsNullOrWhiteSpace(routeOrHeaderTenantId)
                ? AnonymousUserContext.Instance
                : new UserContext(routeOrHeaderTenantId, null, null, null, null);
        }

        var language = await this.userSettingsCache.GetLanguageAsync(userId);

        return new UserContext(
            tenantId: tenantId,
            userId: userId,
            roleKey: roleName != null ? RoleHelper.GetRoleKeyByName(roleName) : null,
            timeZone: timeZone,
            language: language
        );
    }

    public IUserContext Create()
        => CreateAsync().GetAwaiter().GetResult();

    public IUserContext GetUserContextForB2b()
    {
        if (this.httpContextAccessor.HttpContext == null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid,"b2b"),
            };
            var token = this.jwtService.GenerateAccessToken(claims);

            return new UserContext(
                tenantId: "tenant_1",
                userId: "system",
                roleKey: 1,
                timeZone: "UTC",
                language: SupportLanguages.EN.ToString()
            );
        }
        else
        {
            return AnonymousUserContext.Instance;
        }
    }
}
