using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Repositories;
using MediatR;
using System.Security.Cryptography;

namespace DH.Application.Common.Queries;

public record ValidateTenantSetupTokenQuery(string? Token) : IRequest<bool>;

internal class ValidateTenantSetupTokenQueryHandler(
    IRepository<TenantSetupToken> tenantSetupTokenRepository,
    IRepository<TenantApplication> tenantApplicationRepository) : IRequestHandler<ValidateTenantSetupTokenQuery, bool>
{
    readonly IRepository<TenantSetupToken> tenantSetupTokenRepository = tenantSetupTokenRepository;
    readonly IRepository<TenantApplication> tenantApplicationRepository = tenantApplicationRepository;

    public async Task<bool> Handle(ValidateTenantSetupTokenQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
            return true;

        var tokenHash = HashToken(request.Token);
        var setupToken = await this.tenantSetupTokenRepository.GetByAsync(
            x => x.TokenHash == tokenHash && x.UsedAt == null && x.ExpiresAt > DateTime.UtcNow,
            cancellationToken);

        if (setupToken is null)
            return false;

        var application = await this.tenantApplicationRepository.GetByAsync(
            x => x.Id == setupToken.TenantApplicationId,
            cancellationToken);

        return application?.Status == TenantApplicationStatus.Verified
            && string.Equals(application.Email.Trim(), setupToken.Email, StringComparison.OrdinalIgnoreCase);
    }

    static string HashToken(string token)
    {
        var bytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}
