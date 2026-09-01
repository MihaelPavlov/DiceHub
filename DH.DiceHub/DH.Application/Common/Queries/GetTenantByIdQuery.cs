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
        var result = await this.tenantRepository.GetWithPropertiesAsync(
            x => x.Id == request.TenantId,
            x => new GetTenantListQueryModel
            {
                Id = x.Id,
                TenantName = x.TenantName,
                LogoFileName = x.LogoFileName,
                ContactName = x.TenantApplication == null ? string.Empty : x.TenantApplication.ContactName,
                Email = x.TenantApplication == null ? string.Empty : x.TenantApplication.Email,
                PhoneNumber = x.TenantApplication == null ? string.Empty : x.TenantApplication.PhoneNumber,
                Address = x.TenantApplication == null ? x.Town : x.TenantApplication.Address,
                PublicWebsite = x.TenantApplication == null ? string.Empty : x.TenantApplication.PublicWebsite,
                SocialPage = x.TenantApplication == null ? string.Empty : x.TenantApplication.SocialPage,
                DiscordServer = x.TenantApplication == null ? string.Empty : x.TenantApplication.DiscordServer,
                TenantStatus = (int)x.TenantStatus,
                CreatedDate = x.CreatedDate,
                AverageMaxCapacity = x.TenantSetting.AverageMaxCapacity,
                StartWorkingHours = x.TenantSetting.StartWorkingHours,
                EndWorkingHours = x.TenantSetting.EndWorkingHours,
                DaysOff = x.TenantSetting.DaysOff,
                ClubPhoneNumber = x.TenantSetting.PhoneNumber
            }, cancellationToken);

        return result.FirstOrDefault();
    }
}
