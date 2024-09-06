using DH.Adapter.Authentication.Entities;
using DH.Domain.Adapters.Authentication.Models;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtService"/> class with the specified dependencies.
    /// </summary>
    /// <param name="userManager"><see cref="UserManager{TUser}"/> for managing user-related operations.</param>
    /// <param name="configuration"><see cref="IConfiguration"/> for accessing application settings.</param>
    public JwtService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        this.userManager = userManager;
        this.configuration = configuration;
    }

    /// <inheritdoc/>
    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var fe_url = configuration.GetValue<string>("Front_End_Application_URL")
               ?? throw new ArgumentException("Front_End_Application_URL was not specified");
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("JWT_SecrectKey") ?? throw new ArgumentException("JWT_SecretKey was not specified")));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var tokeOptions = new JwtSecurityToken(
            issuer: fe_url,
            audience: fe_url,
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: signinCredentials
        );
        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
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

        var username = principal.Identity.Name; //this is mapped to the Name claim by default
        var user = await userManager.FindByNameAsync(username);
        if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            throw new ArgumentNullException("Invalid client request");
        var newAccessToken = GenerateAccessToken(principal.Claims);
        var newRefreshToken = GenerateRefreshToken();
        user.RefreshToken = newRefreshToken;
        await userManager.UpdateAsync(user);

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
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("JWT_SecrectKey") ?? throw new ArgumentException("JWT_SecretKey was not specified")));

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = secretKey,
            ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");
        return principal;
    }
}