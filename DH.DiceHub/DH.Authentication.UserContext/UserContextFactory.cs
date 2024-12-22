using DH.Authentication.UserContext;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace DH.Messaging.HttpClient.UserContext;

public class UserContextFactory : IUserContextFactory
{
    IUserContext? _defaultUserContext;

    readonly ILogger<UserContextFactory> _logger;
    readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextFactory(ILogger<UserContextFactory> logger, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public IUserContext CreateUserContext()
    {
        if (_httpContextAccessor.HttpContext == null)
            return _defaultUserContext ?? UserContext.Empty;

        if (_httpContextAccessor.HttpContext.Request.Headers.Authorization.FirstOrDefault()?.StartsWith("Bearer ") ?? false)
        {
            try
            {
                int roleKey;
                string? accessToken;
                var userId = _httpContextAccessor.HttpContext.Request.Headers["B2Bsub"].FirstOrDefault();
                if (userId != null)
                {
                    roleKey = (int)Role.Anonymous;
                    accessToken = _httpContextAccessor.HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Substring(7);
                }
                else
                {
                    roleKey = (int)Role.Anonymous;
                    accessToken = _httpContextAccessor.HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Substring(7);
                }

                return new UserContext(userId: userId,
                    roleKey: roleKey,
                    accessToken: accessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bearer impersonation");
                throw;
            }
        }

        #region introduce a separate scope
        {
            var user = _httpContextAccessor.HttpContext.User;

            var userIdparsed = user.FindFirstValue(ClaimTypes.Sid);
            var roleKeyParsed = int.TryParse(user.FindFirstValue(ClaimTypes.Role), out int roleKey);

            return new UserContext(userId: userIdparsed ?? null,
                roleKey: roleKeyParsed ? roleKey : null,
                accessToken: user.FindFirstValue("accessToken"));
        }
        #endregion introduce a separate scope
    }

    public void SetDefaultUserContext(IUserContext defaultUserContext)
    {
        _defaultUserContext = defaultUserContext;
    }
}
