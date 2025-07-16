using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Models.Common;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.Common.Commands;

public record UpdateUserSettingsCommand(UserSettingsDto Settings, string? UserId = null) : IRequest;

internal class UpdateUserSettingsCommandHandler(IRepository<TenantUserSetting> repository, IUserContext userContext) : IRequestHandler<UpdateUserSettingsCommand>
{
    readonly IRepository<TenantUserSetting> repository = repository;
    readonly IUserContext userContext = userContext;

    public async Task Handle(UpdateUserSettingsCommand request, CancellationToken cancellationToken)
    {
        if (!request.Settings.FieldsAreValid(out var validationErrors))
            throw new ValidationErrorsException(validationErrors);

        if (request.Settings.Id == null)
        {
            await this.repository.AddAsync(new TenantUserSetting
            {
                UserId = this.userContext.UserId,
                PhoneNumber = request.Settings.PhoneNumber,
            }, cancellationToken);

            return;
        }

        var currentUserId = request.UserId != null ? request.UserId : this.userContext.UserId;

        var dbSettings = await this.repository.GetByAsyncWithTracking(x => x.Id == request.Settings.Id && currentUserId == x.UserId, cancellationToken);

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

        await this.repository.SaveChangesAsync(cancellationToken);
    }
}