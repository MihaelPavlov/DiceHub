using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DH.Adapter.Authentication.Services;

public class UserService : IUserService
{
    readonly SignInManager<IdentityUser> signInManager;
    readonly UserManager<IdentityUser> userManager;
    readonly IConfiguration configuration;

    public UserService(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        this.signInManager = signInManager;
        this.userManager = userManager;
        this.configuration = configuration;
    }

    public async Task<AuthenticatedResponse> Login(LoginForm form)
    {
        var user = await this.userManager.FindByEmailAsync(form.Email);
        if (user is null)
            throw new ArgumentNullException("User is not found");

        var result = await this.signInManager.PasswordSignInAsync(user, form.Password, true, false);

        if (result.Succeeded)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid,user.Id),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var fe_url = this.configuration.GetValue<string>("Front_End_Application_URL")
                ?? throw new ArgumentException("Front_End_Application_URL was not specified");
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration.GetValue<string>("JWT_SecrectKey") ?? throw new ArgumentException("JWT_SecretKey was not specified")));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokeOptions = new JwtSecurityToken(
                issuer: fe_url,
                audience: fe_url,
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: signinCredentials
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return new AuthenticatedResponse { Token = tokenString };
        }
        return new AuthenticatedResponse { Token = null };
    }

    public async Task Register(RegisterForm form)
    {
        var user = new IdentityUser() { UserName = form.Username, Email = form.Email, EmailConfirmed = true };
        var createUserResult = await userManager.CreateAsync(user, form.Password);
        if (!createUserResult.Succeeded)
        {
            throw new BadHttpRequestException("Result is not successfully!");
        }

        // Refactore
        await userManager.AddToRoleAsync(user, "Admin");
    }
}

public class LoginForm
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterForm
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
public class AuthenticatedResponse
{
    public string? Token { get; set; }
}