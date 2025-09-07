using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Localization;
using DH.Domain.Entities;
using DH.Domain.Models.Common;
using DH.Domain.Repositories;
using DH.Domain.Services.TenantUserSettingsService;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.Common.Commands;

public record UpdateUserSettingsCommand(UserSettingsDto Settings, string? UserId = null) : IRequest;

internal class UpdateUserSettingsCommandHandler(
    IRepository<TenantUserSetting> repository, IUserContext userContext, IUserSettingsCache userSettingsCache, ILocalizationService localizer)
    : IRequestHandler<UpdateUserSettingsCommand>
{
    readonly IRepository<TenantUserSetting> repository = repository;
    readonly IUserContext userContext = userContext;
    readonly IUserSettingsCache userSettingsCache = userSettingsCache;
    readonly ILocalizationService localizer = localizer;

    public async Task Handle(UpdateUserSettingsCommand request, CancellationToken cancellationToken)
    {
        if (!request.Settings.FieldsAreValid(out var validationErrors, localizer))
            throw new ValidationErrorsException(validationErrors);

        if (request.Settings.Id == null && request.UserId == null)
        {
            await this.repository.AddAsync(new TenantUserSetting
            {
                UserId = this.userContext.UserId,
                PhoneNumber = request.Settings.PhoneNumber,
            }, cancellationToken);

            return;
        }

        var currentUserId = request.UserId != null ? request.UserId : this.userContext.UserId;

        TenantUserSetting? dbSettings = null;

        if (request.Settings.Id != null)
            dbSettings = await this.repository.GetByAsyncWithTracking(x => x.Id == request.Settings.Id || currentUserId == x.UserId, cancellationToken);
        else
            dbSettings = await this.repository.GetByAsyncWithTracking(x => currentUserId == x.UserId, cancellationToken);

        if (dbSettings == null)
        {
            await this.repository.AddAsync(new TenantUserSetting
            {
                UserId = currentUserId,
                PhoneNumber = request.Settings.PhoneNumber,
            }, cancellationToken);

            return;
        }

        if (dbSettings!.PhoneNumber != request.Settings.PhoneNumber)
        {
            dbSettings.PhoneNumber = request.Settings.PhoneNumber;
        }

        if (dbSettings!.Language != request.Settings.Language)
        {
            dbSettings.Language = request.Settings.Language;
            this.userSettingsCache.InvalidateLanguage(this.userContext.UserId);
        }

        await this.repository.SaveChangesAsync(cancellationToken);
    }
}