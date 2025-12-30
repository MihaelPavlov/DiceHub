using DH.Domain.Entities;
using DH.Domain.Models.SpaceManagementModels.Queries;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.SpaceManagement.Queries;

public record GetSpaceAvailableTableListQuery(string searchExpressionName) : IRequest<List<GetSpaceAvailableTableListQueryModel>>;

internal class GetSpaceAvailableTableListQueryHandler : IRequestHandler<GetSpaceAvailableTableListQuery, List<GetSpaceAvailableTableListQueryModel>>
{
    readonly IRepository<SpaceTable> repository;

    public GetSpaceAvailableTableListQueryHandler(IRepository<SpaceTable> repository)
    {
        this.repository = repository;
    }

    public async Task<List<GetSpaceAvailableTableListQueryModel>> Handle(GetSpaceAvailableTableListQuery request, CancellationToken cancellationToken)
    {
        return await this.repository.GetWithPropertiesAsync(
            x =>
                (
                x.Name.ToLower().Contains(request.searchExpressionName.ToLower()) ||
                x.Game.Name.ToLower().Contains(request.searchExpressionName.ToLower())
                ) &&
                x.IsTableActive && !x.IsSoloModeActive,
            x => new GetSpaceAvailableTableListQueryModel
            {
                Id = x.Id,
                GameImageUrl = x.Game.ImageUrl,
                GameName = x.Game.Name,
                IsLocked = !string.IsNullOrEmpty(x.Password),
                MaxPeople = x.MaxPeople,
                PeopleJoined = x.SpaceTableParticipants.Count,
                TableName = x.Name,
                CreatedBy = x.CreatedBy,
            }, cancellationToken);
    }
}