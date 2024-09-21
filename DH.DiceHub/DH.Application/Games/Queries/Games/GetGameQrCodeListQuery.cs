using DH.Domain.Entities;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Games.Queries.Games;

public record GetGameQrCodeListQuery(int Id) : IRequest<List<GetGameQrCodeListQueryModel>>;

internal class GetGameQrCodeListQueryHandler : IRequestHandler<GetGameQrCodeListQuery, List<GetGameQrCodeListQueryModel>>
{
    readonly IRepository<GameQrCode> repository;

    public GetGameQrCodeListQueryHandler(IRepository<GameQrCode> repository)
    {
        this.repository = repository;
    }

    public Task<List<GetGameQrCodeListQueryModel>> Handle(GetGameQrCodeListQuery request, CancellationToken cancellationToken)
    {
        return this.repository.GetWithPropertiesAsync(
            x => x.GameId == request.Id,
            x => new GetGameQrCodeListQueryModel
            {
                Id = x.Id,
                GameId = x.GameId,
                FileName = x.FileName,
            }, cancellationToken);
    }
}
