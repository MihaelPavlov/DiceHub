using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Models.Enums;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json;

namespace DH.Adapter.Authentication.Helper;

public class UserContextFactory : IUserContextFactory
{
    readonly IHttpContextAccessor _httpContextAccessor;
    readonly IUserContext _defaultUserContext;
    readonly HttpClient client;
    public UserContextFactory(IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory)
    {
        client = httpClientFactory.CreateClient();
        _httpContextAccessor = httpContextAccessor;
        _defaultUserContext = new UserContext(null, null);
    }

    public IUserContext CreateUserContext()
    {
        if (_httpContextAccessor.HttpContext == null)
            return _defaultUserContext;

        //if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
        //{
        //    var accessToken = authHeader.ToString().Split(' ').Last();
        //    client.DefaultRequestHeaders.Add("Authorization", authHeader.ToString());

        //    var response = client
        //        .GetAsync($"https://localhost:7024/user/info").Result;

        //    if (response.StatusCode != System.Net.HttpStatusCode.OK)
        //    {
        //        return new UserContext(null, null);
        //    }

        //    if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //    {
        //        var content = response.Content.ReadAsStringAsync().Result;
        //        var claims = JsonSerializer.Deserialize<IDictionary<string, string>>(content);
        //        var claimsIdentity = new ClaimsIdentity(claims.Select(c => new Claim(c.Key, c.Value)));
        //        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        //        _httpContextAccessor.HttpContext.User = claimsPrincipal;
        //    }
        //}

        var user = _httpContextAccessor.HttpContext.User;

        var userIdClaim = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid);
        var userRoleClaim = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);

        return new UserContext(userId: userIdClaim != null ? userIdClaim.Value : null, roleKey: userRoleClaim != null ? RoleHelper.GetRoleKeyByName(userRoleClaim.Value) : null);
    }
}
