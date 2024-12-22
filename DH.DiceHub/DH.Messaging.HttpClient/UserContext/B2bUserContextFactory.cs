using DH.Messaging.HttpClient.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace DH.Messaging.HttpClient.UserContext;

public class B2bUserContextFactory : IB2bUserContextFactory
{
    IB2bUserContext? _defaultUserContext;

    readonly ILogger<B2bUserContextFactory> _logger;
    readonly IHttpContextAccessor _httpContextAccessor;

    public B2bUserContextFactory(ILogger<B2bUserContextFactory> logger, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public IB2bUserContext CreateUserContext()
    {
        if (_httpContextAccessor.HttpContext == null)
            return _defaultUserContext ?? B2bUserContext.Empty;

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

                return new B2bUserContext(userId: userId,
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

            return new B2bUserContext(userId: userIdparsed ?? null,
                roleKey: roleKeyParsed ? roleKey : null,
                accessToken: user.FindFirstValue("accessToken"));
        }
        #endregion introduce a separate scope
    }

    public void SetDefaultUserContext(IB2bUserContext defaultUserContext)
    {
        _defaultUserContext = defaultUserContext;
    }
}
