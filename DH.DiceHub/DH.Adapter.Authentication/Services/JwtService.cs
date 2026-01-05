using DH.Adapter.Authentication.Entities;
using DH.Domain.Adapters.Authentication.Models;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Adapters.Authentication.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DH.Adapter.Authentication.Services;

/// <summary>
/// Implements the <see cref="IJwtService"/>. 
/// Provides methods for generating and refreshing JWT tokens.
/// </summary>
public class JwtService : IJwtService
{
    readonly UserManager<ApplicationUser> userManager;
    readonly IConfiguration configuration;
    readonly IPermissionStringBuilder _permissionStringBuilder;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtService"/> class with the specified dependencies.
    /// </summary>
    /// <param name="userManager"><see cref="UserManager{TUser}"/> for managing user-related operations.</param>
    /// <param name="configuration"><see cref="IConfiguration"/> for accessing application settings.</param>
    public JwtService(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        IPermissionStringBuilder permissionStringBuilder
    )
    {
        this.userManager = userManager;
        this.configuration = configuration;
        _permissionStringBuilder = permissionStringBuilder;
    }

    /// <inheritdoc/>
    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var apiAudiences = configuration.GetSection("APIs_Audience_URLs").Get<string[]>()
            ?? throw new ArgumentException("APIs_Audience_URLs was not specified");

        var issuer = configuration.GetValue<string>("TokenIssuer")
            ?? throw new ArgumentException("TokenIssuer was not specified");

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("JWT_SecretKey")
            ?? throw new ArgumentException("JWT_SecretKey was not specified")));

        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var payload = new JwtPayload(
            issuer: issuer,
            audience: null,
            claims: claims,
            notBefore: null,
            expires: DateTime.UtcNow.AddDays(1)
        );

        payload["aud"] = apiAudiences;

        var token = new JwtSecurityToken(new JwtHeader(signinCredentials), payload);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenString;
    }

    /// <inheritdoc/>
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    /// <inheritdoc/>
    public async Task<TokenResponseModel> RefreshAccessTokenAsync(TokenResponseModel tokens)
    {
        if (tokens is null)
            throw new ArgumentNullException("Invalid client request");

        string accessToken = tokens.AccessToken;
        string refreshToken = tokens.RefreshToken;

        var principal = GetPrincipalFromExpiredToken(accessToken);
        var userId = principal.FindFirstValue(ClaimTypes.Sid);

        if (string.IsNullOrEmpty(userId))
            throw new SecurityTokenException("Invalid token");

        var user = await userManager.FindByIdAsync(userId);

        if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            throw new SecurityTokenException("Invalid refresh token");

        var roles = await userManager.GetRolesAsync(user);
        var roleName = roles.FirstOrDefault();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Sid, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim("TimeZone", user.TimeZone!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        if (roleName != null)
        {
            claims.Add(new Claim(ClaimTypes.Role, roleName));
            claims.Add(new Claim("role_key",
                RoleHelper.GetRoleKeyByName(roleName).ToString()));

            claims.Add(new Claim(
                "permissions",
                this._permissionStringBuilder
                    .GetFromCacheOrBuildPermissionsString(
                        RoleHelper.GetRoleKeyByName(roleName))));

        }

        var newAccessToken = GenerateAccessToken(claims);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(5);
        user.SecurityStamp = Guid.NewGuid().ToString();

        /*
         SecurityStamp is not a refresh-token field.
            It is an identity invalidation marker.
            ASP.NET Identity uses it to:
            invalidate cookies
            invalidate tokens
            force re-authentication across sessions
            Think of it as “kill all sessions”.
         */

        await this.userManager.UpdateAsync(user);

        return new TokenResponseModel()
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    public ClaimsPrincipal ValidateToken(string accessToken)
    {
        return this.GetPrincipalFromExpiredToken(accessToken);
    }

    /// <summary>
    /// Retrieves the principal from an expired token.
    /// </summary>
    /// <param name="token">The expired token.</param>
    /// <returns>A <see cref="ClaimsPrincipal"/> representing the user.</returns>
    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var apiAudiences = configuration.GetSection("APIs_Audience_URLs").Get<string[]>()
            ?? throw new ArgumentException("APIs_Audience_URLs was not specified");

        var issuer = configuration.GetValue<string>("TokenIssuer")
            ?? throw new ArgumentException("TokenIssuer was not specified");

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("JWT_SecretKey")
            ?? throw new ArgumentException("JWT_SecretKey was not specified")));

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,

            ValidateLifetime = false,
            // Prevents accepting tokens that are “almost expired”
            ClockSkew = TimeSpan.Zero,  //Keeps refresh logic deterministic
                                        // Avoids subtle timing bugs during rotation
            ValidIssuer = issuer,
            ValidAudiences = apiAudiences,
            IssuerSigningKey = secretKey,
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var principal = tokenHandler.ValidateToken(
            token,
            tokenValidationParameters,
            out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwt ||
        !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
            StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }
}