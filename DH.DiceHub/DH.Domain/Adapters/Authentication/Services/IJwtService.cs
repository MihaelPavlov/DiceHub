using DH.Domain.Adapters.Authentication.Models;
using System.Security.Claims;

namespace DH.Domain.Adapters.Authentication.Services;

/// <summary>
/// Defines methods for generating and refreshing JWT tokens.
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generates a new access token based on the provided claims.
    /// </summary>
    /// <param name="claims">The claims to include in the token.</param>
    /// <returns>A JWT access token as a string.</returns>
    string GenerateAccessToken(IEnumerable<Claim> claims);

    /// <summary>
    /// Refreshes the access token using the provided token model.
    /// </summary>
    /// <param name="tokens">The token model containing the access and refresh tokens.</param>
    /// <returns>A new <see cref="TokenResponseModel"/> with refreshed tokens.</returns>
    Task<TokenResponseModel> RefreshAccessTokenAsync(TokenResponseModel tokens);

    /// <summary>
    /// Generates a new refresh token.
    /// </summary>
    /// <returns>A refresh token as a string.</returns>
    string GenerateRefreshToken();
    ClaimsPrincipal ValidateToken(string accessToken);
}
