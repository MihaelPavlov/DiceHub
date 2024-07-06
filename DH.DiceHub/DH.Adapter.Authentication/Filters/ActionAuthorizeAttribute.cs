using DH.Domain.Adapters.Authentication.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DH.Adapter.Authentication.Filters;

public class ActionAuthorizeAttribute : TypeFilterAttribute
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userAction"></param>
    public ActionAuthorizeAttribute(UserAction userAction) : base(typeof(ActionAuthorizeFilter))
    {
        Arguments = [(int)userAction];
    }
}
