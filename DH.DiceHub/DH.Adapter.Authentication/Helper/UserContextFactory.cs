using DH.Domain.Adapters.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DH.Adapter.Authentication.Helper;

public class UserContextFactory : IUserContextFactory
{
    readonly IHttpContextAccessor _httpContextAccessor;
    readonly IUserContext _defaultUserContext;

    public UserContextFactory(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _defaultUserContext = new UserContext(null);
    }

    public IUserContext CreateUserContext()
    {
        if (_httpContextAccessor.HttpContext == null)
            return _defaultUserContext;

        var user = _httpContextAccessor.HttpContext.User;

        var userIdClaim = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid);

        return new UserContext(userId: userIdClaim != null ? userIdClaim.Value : null);
    }
}
