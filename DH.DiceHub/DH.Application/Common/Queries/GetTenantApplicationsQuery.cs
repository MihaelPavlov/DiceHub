using DH.Domain.Entities;
using DH.Domain.Models.Common;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Common.Queries;

public record GetTenantApplicationsQuery : IRequest<List<TenantApplicationDto>>;

internal class GetTenantApplicationsQueryHandler(IRepository<TenantApplication> repository)
    : IRequestHandler<GetTenantApplicationsQuery, List<TenantApplicationDto>>
{
    readonly IRepository<TenantApplication> repository = repository;

    public async Task<List<TenantApplicationDto>> Handle(GetTenantApplicationsQuery request, CancellationToken cancellationToken)
    {
        var applications = await repository.GetWithPropertiesAsync(x => new TenantApplicationDto
        {
            Id = x.Id,
            ApplicantType = x.ApplicantType,
            ContactName = x.ContactName,
            Email = x.Email,
            PhoneNumber = x.PhoneNumber,
            IsEmailVerified = x.IsEmailVerified,
            IsPhoneVerified = x.IsPhoneVerified,
            Address = x.Address,
            PublicWebsite = x.PublicWebsite,
            SocialPage = x.SocialPage,
            DiscordServer = x.DiscordServer,
            PhotoUrl = x.PhotoUrl,
            Status = x.Status,
            CreatedDate = x.CreatedDate,
            ReviewedDate = x.ReviewedDate,
            ReviewedByUserId = x.ReviewedByUserId,
            ReviewNote = x.ReviewNote,
        }, cancellationToken);

        return applications
            .OrderBy(x => x.Status)
            .ThenByDescending(x => x.CreatedDate)
            .ToList();
    }
}
