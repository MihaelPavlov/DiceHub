using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Localization;
using DH.Domain.Entities;
using DH.Domain.Models.Common;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace DH.Application.Common.Commands;

public record CreateTenantApplicationCommand(TenantApplicationRequest Application) : IRequest<int>;

internal class CreateTenantApplicationCommandHandler(
    IRepository<TenantApplication> repository,
    ISystemUserContextAccessor systemUserContextAccessor,
    ILocalizationService localizer,
    IMemoryCache memoryCache) : IRequestHandler<CreateTenantApplicationCommand, int>
{
    readonly IRepository<TenantApplication> repository = repository;
    readonly ISystemUserContextAccessor systemUserContextAccessor = systemUserContextAccessor;
    readonly ILocalizationService localizer = localizer;
    readonly IMemoryCache memoryCache = memoryCache;

    public async Task<int> Handle(CreateTenantApplicationCommand request, CancellationToken cancellationToken)
    {
        if (!request.Application.FieldsAreValid(out var validationErrors, localizer))
            throw new ValidationErrorsException(validationErrors);

        var normalizedEmail = request.Application.Email.Trim().ToLowerInvariant();
        if (!this.memoryCache.TryGetValue<bool>(TenantApplicationEmailVerificationCache.BuildVerifiedKey(normalizedEmail), out var isEmailVerified) || !isEmailVerified)
            throw new ValidationErrorsException(nameof(request.Application.Email), "Email verification has expired. Verify the email again.");

        systemUserContextAccessor.Set(new TenantApplicationSystemUserContext());

        var result = await repository.AddAsync(new TenantApplication
        {
            ApplicantType = request.Application.ApplicantType,
            ContactName = request.Application.ContactName,
            Email = normalizedEmail,
            PhoneNumber = request.Application.PhoneNumber,
            IsEmailVerified = request.Application.IsEmailVerified,
            IsPhoneVerified = request.Application.IsPhoneVerified,
            Address = request.Application.Address,
            PublicWebsite = request.Application.PublicWebsite,
            SocialPage = request.Application.SocialPage,
            DiscordServer = request.Application.DiscordServer,
            PhotoUrl = request.Application.PhotoUrl,
            CreatedDate = DateTime.UtcNow,
        }, cancellationToken);

        this.memoryCache.Remove(TenantApplicationEmailVerificationCache.BuildVerifiedKey(normalizedEmail));

        return result.Id;
    }

    private sealed class TenantApplicationSystemUserContext : IUserContext
    {
        public string? TenantId => null;
        public string? UserId => "tenant-application";
        public int? RoleKey => null;
        public string? TimeZone => "UTC";
        public string? Language => "en";
        public bool IsAuthenticated => false;
        public bool IsSystem => true;
    }
}
