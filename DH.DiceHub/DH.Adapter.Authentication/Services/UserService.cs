using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DH.Adapter.Authentication.Services;

public class UserService : IUserService
{
    public Dictionary<string, string> GetUser(ClaimsPrincipal user)
    => user.Claims.ToDictionary(x => x.Type, x => x.Value);

    public IResult Login(LoginForm form)
    {
        return Results.SignIn(
             new ClaimsPrincipal(
                 new ClaimsIdentity(
                     new Claim[]
                     {
                        new Claim("user_id", Guid.NewGuid().ToString()),
                        new Claim("username" , form.Username)
                     },
                     "cookie")
                 ),
             properties: new Microsoft.AspNetCore.Authentication.AuthenticationProperties() { IsPersistent = true },
             authenticationScheme: "cookie");
    }

    public void Register()
    {

    }

    public void Logout()
    {
        Results.SignOut(authenticationSchemes: new List<string> { "cookie" });
    }
}

public class LoginForm
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}