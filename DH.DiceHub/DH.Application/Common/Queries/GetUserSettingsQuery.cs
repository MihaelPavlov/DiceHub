using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Models.Common;
using DH.Domain.Repositories;
using Mapster;
using MediatR;

namespace DH.Application.Common.Queries;

public record GetUserSettingsQuery : IRequest<UserSettingsDto>;

internal class GetUserSettingsQueryHandler(IRepository<TenantUserSetting> repository, IUserContext userContext) : IRequestHandler<GetUserSettingsQuery, UserSettingsDto>
{
    readonly IRepository<TenantUserSetting> repository = repository;
    readonly IUserContext userContext = userContext;

    public async Task<UserSettingsDto> Handle(GetUserSettingsQuery request, CancellationToken cancellationToken)
    {
        var settings = await this.repository.GetByAsync(x => x.UserId == this.userContext.UserId, cancellationToken);
        return settings.Adapt<UserSettingsDto>();
    }
}