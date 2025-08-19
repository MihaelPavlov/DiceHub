using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Models.Common;
using DH.Domain.Repositories;
using MediatR;
using System.Text.Json;

namespace DH.Application.Common.Queries;

public record GetAssistiveTouchSettingsQuery : IRequest<AssistiveTouchSettings>;

internal class GetAssistiveTouchSettingsQueryHandler(IUserContext userContext, IRepository<TenantUserSetting> repository) : IRequestHandler<GetAssistiveTouchSettingsQuery, AssistiveTouchSettings>
{
    readonly IUserContext userContext = userContext;
    readonly IRepository<TenantUserSetting> repository = repository;

    public async Task<AssistiveTouchSettings> Handle(GetAssistiveTouchSettingsQuery request, CancellationToken cancellationToken)
    {
        var userSettings = await this.repository.GetByAsync(x => x.UserId == this.userContext.UserId, cancellationToken);

        if (userSettings == null)
            return new AssistiveTouchSettings();

        if (string.IsNullOrEmpty(userSettings.AssistiveTouchSettings))
        {
            return new AssistiveTouchSettings();
        }

        return JsonSerializer.Deserialize<AssistiveTouchSettings>(userSettings.AssistiveTouchSettings)
            ?? throw new Exception("Couldn't load your assistive touch settings. Please ensure your settings are correctly set up");
    }
}