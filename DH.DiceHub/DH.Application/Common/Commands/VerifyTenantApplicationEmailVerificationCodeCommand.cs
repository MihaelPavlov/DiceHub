using DH.Domain.Models.Common;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;

namespace DH.Application.Common.Commands;

public record VerifyTenantApplicationEmailVerificationCodeCommand(TenantApplicationVerifyEmailCodeRequest Request) : IRequest<bool>;

internal class VerifyTenantApplicationEmailVerificationCodeCommandHandler(
    IMemoryCache memoryCache) : IRequestHandler<VerifyTenantApplicationEmailVerificationCodeCommand, bool>
{
    const int MaxAttempts = 5;

    readonly IMemoryCache memoryCache = memoryCache;

    public Task<bool> Handle(VerifyTenantApplicationEmailVerificationCodeCommand request, CancellationToken cancellationToken)
    {
        var email = request.Request.Email.Trim().ToLowerInvariant();
        var code = request.Request.Code.Trim();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(code))
            return Task.FromResult(false);

        var cacheKey = TenantApplicationEmailVerificationCache.BuildKey(email);
        if (!this.memoryCache.TryGetValue<TenantApplicationEmailVerificationCache.Entry>(cacheKey, out var entry) || entry is null)
            return Task.FromResult(false);

        if (entry.CodeHash == HashCode(code))
        {
            this.memoryCache.Remove(cacheKey);
            this.memoryCache.Set(TenantApplicationEmailVerificationCache.BuildVerifiedKey(email), true, TimeSpan.FromMinutes(30));
            return Task.FromResult(true);
        }

        var attempts = entry.Attempts + 1;
        if (attempts >= MaxAttempts)
        {
            this.memoryCache.Remove(cacheKey);
            return Task.FromResult(false);
        }

        this.memoryCache.Set(cacheKey, entry with { Attempts = attempts }, TimeSpan.FromMinutes(10));
        return Task.FromResult(false);
    }

    static string HashCode(string code)
    {
        var bytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(code));
        return Convert.ToHexString(bytes);
    }
}
