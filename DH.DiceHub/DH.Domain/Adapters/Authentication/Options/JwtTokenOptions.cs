namespace DH.Domain.Adapters.Authentication.Options;

public sealed class JwtTokenOptions
{
    public required string Issuer { get; init; }
    public required string[] Audiences { get; init; }
    public required byte[] SigningKey { get; init; }
    public TimeSpan AccessTokenLifetime { get; init; } = TimeSpan.FromDays(1);
    public TimeSpan RefreshTokenLifetime { get; init; } = TimeSpan.FromDays(7);
}
