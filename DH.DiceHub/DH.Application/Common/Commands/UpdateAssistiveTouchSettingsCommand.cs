using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Models.Common;
using DH.Domain.Repositories;
using MediatR;
using System.Text.Json;

namespace DH.Application.Common.Commands;

public record UpdateAssistiveTouchSettingsCommand(AssistiveTouchSettings Payload) : IRequest;

internal class UpdateAssistiveTouchSettingsCommandHandler(IUserContext userContext, IRepository<TenantUserSetting> repository) : IRequestHandler<UpdateAssistiveTouchSettingsCommand>
{
    readonly IUserContext userContext = userContext;
    readonly IRepository<TenantUserSetting> repository = repository;

    public async Task Handle(UpdateAssistiveTouchSettingsCommand request, CancellationToken cancellationToken)
    {
        var userSettings = await this.repository.GetByAsyncWithTracking(x => x.UserId == this.userContext.UserId, cancellationToken);

        if (userSettings == null)
        {
            await this.repository.AddAsync(new TenantUserSetting
            {
                UserId = this.userContext.UserId,
                AssistiveTouchSettings = JsonSerializer.Serialize(request.Payload)
            }, cancellationToken);
        }
        else
        {
            userSettings.AssistiveTouchSettings = JsonSerializer.Serialize(request.Payload);
            await this.repository.SaveChangesAsync(cancellationToken);
        }
    }
}