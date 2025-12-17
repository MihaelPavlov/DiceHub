using DH.Domain.Entities;
using DH.Domain.Models.Common;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Common.Queries;

public record GetTenantListQuery : IRequest<List<GetTenantListQueryModel>>;

internal class GetTenantListQueryHandler(IRepository<Tenant> tenantRepository) : IRequestHandler<GetTenantListQuery, List<GetTenantListQueryModel>>
{
    readonly IRepository<Tenant> tenantRepository = tenantRepository;

    public async Task<List<GetTenantListQueryModel>> Handle(GetTenantListQuery request, CancellationToken cancellationToken)
    {
        return await this.tenantRepository.GetWithPropertiesAsync(x => new GetTenantListQueryModel
        {
            Id = x.Id,
            TenantName = x.TenantName,
            LogoFileName = x.LogoFileName
        }, cancellationToken);
    }
}