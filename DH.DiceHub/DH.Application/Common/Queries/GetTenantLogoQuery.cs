using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.Common.Queries;

public record GetTenantLogoQuery : IRequest<string?>;

internal class GetTenantLogoQueryHandler(
    IRepository<Tenant> tenantRepository,
    IUserContext userContext) : IRequestHandler<GetTenantLogoQuery, string?>
{
    readonly IRepository<Tenant> tenantRepository = tenantRepository;
    readonly IUserContext userContext = userContext;

    public async Task<string?> Handle(GetTenantLogoQuery request, CancellationToken cancellationToken)
    {
        var tenantId = this.userContext.TenantId
            ?? throw new BadRequestException("Tenant context is required to get the club image.");

        var tenant = await this.tenantRepository.GetByAsync(x => x.Id == tenantId, cancellationToken)
            ?? throw new NotFoundException(nameof(Tenant), tenantId);

        return tenant.LogoFileName;
    }
}
