using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DH.Adapter.Authentication.Services;

public interface IUserService
{
    Dictionary<string, string> GetUser(ClaimsPrincipal user);
    IResult Login(LoginForm form);
    void Logout();
    void Register();
}
