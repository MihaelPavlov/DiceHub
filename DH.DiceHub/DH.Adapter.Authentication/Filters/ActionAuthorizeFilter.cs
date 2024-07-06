using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using DH.Domain.Adapters.Authentication.Services;

namespace DH.Adapter.Authentication.Filters;

public class ActionAuthorizeFilter : IAsyncAuthorizationFilter
{
    readonly IUserActionService _userActionService;
    readonly int _actionKey;

    public ActionAuthorizeFilter(IUserActionService userActionService, int actionKey)
    {
        _userActionService = userActionService;
        _actionKey = actionKey;
    }

    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (!_userActionService.IsActionAvailable(_actionKey))
        {
            context.Result = new ForbidResult();
        }

        return Task.CompletedTask;
    }
}
