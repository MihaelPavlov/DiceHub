using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Repositories;
using DH.Domain.Services.TenantUserSettingsService;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DH.Adapter.Authentication.Helper;

public class UserContextFactory : IUserContextFactory
{
    IUserContext _defaultUserContext;

    readonly IHttpContextAccessor _httpContextAccessor;
    readonly HttpClient client;
    readonly IJwtService jwtService;
    readonly IUserSettingsCache userSettingsCache;

    public UserContextFactory(IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory, IJwtService jwtService, IUserSettingsCache userSettingsCache)
    {
        client = httpClientFactory.CreateClient();
        _httpContextAccessor = httpContextAccessor;
        this.jwtService = jwtService;
        this.userSettingsCache = userSettingsCache;
        _defaultUserContext = new UserContext(null, null, null, null, null);
    }

    public IUserContext CreateUserContext()
    {
        if (_httpContextAccessor.HttpContext == null)
            return _defaultUserContext;

        if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            var accessToken = authHeader.ToString().Split(' ').Last();
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", authHeader.ToString());

            var user = _httpContextAccessor.HttpContext.User;

            var userIdClaim = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid);
            string language = SupportLanguages.EN.ToString();
            if (userIdClaim != null)
                language = this.userSettingsCache.GetLanguageAsync(userIdClaim.Value).GetAwaiter().GetResult();


            var userRoleClaim = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);
            var userTimeZone = user.Claims.FirstOrDefault(x => x.Type == "TimeZone");

            var userContext = new UserContext(
                userId: userIdClaim != null ? userIdClaim.Value : null,
                roleKey: userRoleClaim != null ? RoleHelper.GetRoleKeyByName(userRoleClaim.Value) : null,
                accessToken,
                userTimeZone?.Value ?? null,
                language);
            this._defaultUserContext = userContext;
            return userContext;
        }
        return this._defaultUserContext;
    }

    public void SetDefaultUserContext(IUserContext defaultUserContext)
    {
        _defaultUserContext = defaultUserContext;
    }

    public IUserContext GetUserContextForB2b()
    {
        if (_httpContextAccessor.HttpContext == null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid,"b2b"),
            };
            var token = this.jwtService.GenerateAccessToken(claims);

            return new UserContext("1", 1, token, "timezone", SupportLanguages.EN.ToString());
        }
        else
        {
            return CreateUserContext();
        }
    }
}
