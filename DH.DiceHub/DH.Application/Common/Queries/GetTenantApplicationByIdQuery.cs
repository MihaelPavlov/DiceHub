using DH.Domain.Entities;
using DH.Domain.Models.Common;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Common.Queries;

public record GetTenantApplicationByIdQuery(int Id) : IRequest<TenantApplicationDto?>;

internal class GetTenantApplicationByIdQueryHandler(IRepository<TenantApplication> repository)
    : IRequestHandler<GetTenantApplicationByIdQuery, TenantApplicationDto?>
{
    readonly IRepository<TenantApplication> repository = repository;

    public async Task<TenantApplicationDto?> Handle(GetTenantApplicationByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetByAsync(x => x.Id == request.Id, cancellationToken);

        if (result is null)
            return null;

        return new TenantApplicationDto
        {
            Id = result.Id,
            ApplicantType = result.ApplicantType,
            ContactName = result.ContactName,
            Email = result.Email,
            PhoneNumber = result.PhoneNumber,
            IsEmailVerified = result.IsEmailVerified,
            IsPhoneVerified = result.IsPhoneVerified,
            Address = result.Address,
            PublicWebsite = result.PublicWebsite,
            SocialPage = result.SocialPage,
            DiscordServer = result.DiscordServer,
            PhotoUrl = result.PhotoUrl,
            Status = result.Status,
            CreatedDate = result.CreatedDate,
            ReviewedDate = result.ReviewedDate,
            ReviewedByUserId = result.ReviewedByUserId,
            ReviewNote = result.ReviewNote,
        };
    }
}
