using DH.Domain.Entities;
using DH.Domain.Models.Common;
using DH.Domain.Repositories;
using MediatR;
namespace DH.Application.Common.Queries;

public record GetTenantByIdQuery(string TenantId) : IRequest<GetTenantListQueryModel?>;

internal class GetTenantByIdQueryHandler(IRepository<Tenant> tenantRepository) : IRequestHandler<GetTenantByIdQuery, GetTenantListQueryModel?>
{
    readonly IRepository<Tenant> tenantRepository = tenantRepository;

    public async Task<GetTenantListQueryModel?> Handle(GetTenantByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await this.tenantRepository
            .GetByAsync(x => x.Id == request.TenantId, cancellationToken);

        if (result == null) return null;

        return new GetTenantListQueryModel
        {
            Id = result.Id,
            TenantName = result.TenantName,
            LogoFileName = result.LogoFileName
        };
    }
}