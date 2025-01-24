using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Adapters.Authentication.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DH.Adapter.Authentication.Helper;

public class UserContextFactory : IUserContextFactory
{
    IUserContext _defaultUserContext;
    readonly IHttpContextAccessor _httpContextAccessor;
    readonly HttpClient client;
    readonly IJwtService jwtService;

    public UserContextFactory(IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory, IJwtService jwtService)
    {
        client = httpClientFactory.CreateClient();
        _httpContextAccessor = httpContextAccessor;
        this.jwtService = jwtService;
        _defaultUserContext = new UserContext(null, null, null);
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

            //var response = client
            //    .GetAsync($"https://localhost:7024/user/info").Result;

            //if (response.StatusCode != System.Net.HttpStatusCode.OK)
            //{
            //    return new UserContext(null, null);
            //}

            //if (response.StatusCode == System.Net.HttpStatusCode.OK)
            //{
            //    var content = response.Content.ReadAsStringAsync().Result;
            //    var claims = JsonSerializer.Deserialize<IDictionary<string, string>>(content);
            //    var claimsIdentity = new ClaimsIdentity(claims.Select(c => new Claim(c.Key, c.Value)));
            //    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            //    _httpContextAccessor.HttpContext.User = claimsPrincipal;
            //}
            var user = _httpContextAccessor.HttpContext.User;

            var userIdClaim = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid);
            var userRoleClaim = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);

            var userContext = new UserContext(userId: userIdClaim != null ? userIdClaim.Value : null, roleKey: userRoleClaim != null ? RoleHelper.GetRoleKeyByName(userRoleClaim.Value) : null, accessToken);
            this._defaultUserContext = userContext;
            return userContext;
        }
        return this._defaultUserContext;
        //var user = _httpContextAccessor.HttpContext.User;

        //var userIdClaim = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid);
        //var userRoleClaim = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);

        //var userContext = new UserContext(userId: userIdClaim != null ? userIdClaim.Value : null, roleKey: userRoleClaim != null ? RoleHelper.GetRoleKeyByName(userRoleClaim.Value) : null);
        //this._defaultUserContext = userContext;
        //return userContext;
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

            return new UserContext("1", 1, token); //Todo: it might be needed to generate a valid token, because of the authentication inside the statistic service
        }
        else
        {
            return CreateUserContext();
        }
    }
}
