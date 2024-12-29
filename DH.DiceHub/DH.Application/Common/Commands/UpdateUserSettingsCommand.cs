using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Models.Common;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.Common.Commands;

public record UpdateUserSettingsCommand(UserSettingsDto Settings) : IRequest;

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
                PhoneNumber = request.Settings.PhoneNumber,
            }, cancellationToken);

            return;
        }

        var dbSettings = await this.repository.GetByAsyncWithTracking(x => x.Id == request.Settings.Id && this.userContext.UserId == x.UserId, cancellationToken);

        if (dbSettings!.PhoneNumber != request.Settings.PhoneNumber)
        {
            dbSettings.PhoneNumber = request.Settings.PhoneNumber;
        }

        await this.repository.SaveChangesAsync(cancellationToken);
    }
}