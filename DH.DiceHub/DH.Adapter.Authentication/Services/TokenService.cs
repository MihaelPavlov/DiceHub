using DH.Adapter.Authentication.Entities;
using DH.Domain.Adapters.Authentication.Models;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Adapters.Authentication.Options;
using DH.Domain.Adapters.Authentication.Services;
using DH.OperationResultCore.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace DH.Adapter.Authentication.Services;

/// <summary>
/// Implements the <see cref="ITokenService"/>. 
/// Provides methods for generating and refreshing JWT tokens.
/// </summary>
public class TokenService : ITokenService
{
    readonly UserManager<ApplicationUser> userManager;
    readonly IPermissionStringBuilder permissionStringBuilder;
    readonly JwtTokenOptions jwtTokenOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenService"/> class with the specified dependencies.
    /// </summary>
    public TokenService(
        UserManager<ApplicationUser> userManager,
        IPermissionStringBuilder permissionStringBuilder,
        JwtTokenOptions jwtTokenOptions
    )
    {
        this.userManager = userManager;
        this.permissionStringBuilder = permissionStringBuilder;
        this.jwtTokenOptions = jwtTokenOptions;

    }

    /// <inheritdoc/>
    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var signinCredentials = new SigningCredentials(
            new SymmetricSecurityKey(this.jwtTokenOptions.SigningKey), SecurityAlgorithms.HmacSha256);

        var tokeOptions = new JwtPayload(
            issuer: this.jwtTokenOptions.Issuer,
            audience: null,
            claims: claims,
            notBefore: null,
            expires: DateTime.UtcNow.Add(this.jwtTokenOptions.AccessTokenLifetime)
        );

        tokeOptions["aud"] = this.jwtTokenOptions.Audiences;

        var token = new JwtSecurityToken(new JwtHeader(signinCredentials), tokeOptions);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenString;
    }

    /// <inheritdoc/>
    public string GenerateRefreshToken()
    {
        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    public DateTime GetRefreshTokenExpiryTime()
    {
        return DateTime.UtcNow.Add(this.jwtTokenOptions.RefreshTokenLifetime);
    }

    /// <inheritdoc/>
    public async Task<TokenResponseModel> RefreshAccessTokenAsync(TokenResponseModel tokens)
    {
        if (tokens is null)
            throw new SecurityTokenException("Invalid client request");

        if (string.IsNullOrEmpty(tokens.AccessToken) || string.IsNullOrEmpty(tokens.RefreshToken))
            throw new SecurityTokenException("Invalid client request");

        var principal = GetPrincipalFromExpiredToken(tokens.AccessToken);
        var userId = principal.FindFirstValue(ClaimTypes.Sid);
        var tokenTenantId = principal.FindFirstValue("tenant_id");

        if (string.IsNullOrEmpty(userId))
            throw new SecurityTokenException("Invalid token");

        var user = await userManager.FindByIdAsync(userId);

        if (user is null ||
            user.RefreshToken != tokens.RefreshToken ||
            user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            throw new SecurityTokenException("Invalid refresh token");

        if (tokenTenantId != user.TenantId)
            throw new SecurityTokenException("Tenant mismatch");

        var roles = await userManager.GetRolesAsync(user);
        var roleName = roles.FirstOrDefault();

        var claims = await BuildUserClaimsAsync(user.Id);

        var newAccessToken = GenerateAccessToken(claims);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = this.GetRefreshTokenExpiryTime();

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

    public async Task<List<Claim>> BuildUserClaimsAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException(nameof(ApplicationUser), userId);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Sid, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim("TimeZone", user.TimeZone!),
            new Claim("tenant_id", user.TenantId)
        };

        var roles = await userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault();

        if (role is not null)
        {
            var roleKey = RoleHelper.GetRoleKeyByName(role);

            claims.Add(new Claim(ClaimTypes.Role, role));
            claims.Add(new Claim("role_key", roleKey.ToString()));
            claims.Add(new Claim(
                "permissions",
                permissionStringBuilder.GetFromCacheOrBuildPermissionsString(roleKey)));
        }

        return claims;
    }

    public ClaimsPrincipal ValidateToken(string accessToken)
        => GetPrincipalFromExpiredToken(accessToken);

    /// <summary>
    /// Retrieves the principal from an expired token.
    /// </summary>
    /// <param name="token">The expired token.</param>
    /// <returns>A <see cref="ClaimsPrincipal"/> representing the user.</returns>
    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,

            ValidateLifetime = false,
            // Prevents accepting tokens that are “almost expired”
            ClockSkew = TimeSpan.Zero,  //Keeps refresh logic deterministic
                                        // Avoids subtle timing bugs during rotation
            ValidIssuer = this.jwtTokenOptions.Issuer,
            ValidAudiences = this.jwtTokenOptions.Audiences,
            IssuerSigningKey = new SymmetricSecurityKey(this.jwtTokenOptions.SigningKey),
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