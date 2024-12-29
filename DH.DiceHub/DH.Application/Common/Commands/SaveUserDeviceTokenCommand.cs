using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.Common.Commands;

public record SaveUserDeviceTokenCommand(string DeviceToken) : IRequest;

internal class SaveUserDeviceTokenCommandHandler : IRequestHandler<SaveUserDeviceTokenCommand>
{
    readonly IRepository<UserDeviceToken> repository;
    readonly IUserContext userContext;

    public SaveUserDeviceTokenCommandHandler(IUserContext userContext, IRepository<UserDeviceToken> repository)
    {
        this.userContext = userContext;
        this.repository = repository;
    }

    public async Task Handle(SaveUserDeviceTokenCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.DeviceToken))
            throw new ValidationErrorsException("DeviceToken", "Device Token is required");

        var userDeviceToken = await this.repository.GetByAsyncWithTracking(x => x.UserId == this.userContext.UserId, cancellationToken);

        if (userDeviceToken == null)
        {
            await this.repository.AddAsync(new UserDeviceToken
            {
                DeviceToken = request.DeviceToken,
                UserId = this.userContext.UserId,
                LastUpdated = DateTime.UtcNow,
            }, cancellationToken);
        }
        else
        {
            userDeviceToken.DeviceToken = request.DeviceToken;
            userDeviceToken.LastUpdated = DateTime.UtcNow;

            await this.repository.SaveChangesAsync(cancellationToken);
        }
    }
}