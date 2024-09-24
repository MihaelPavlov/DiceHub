using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Repositories;
using Mapster;
using MediatR;

namespace DH.Application.Games.Queries.Games;

public record GetGameQrCodeQuery(int Id) : IRequest<GetGameQrCodeQueryModel>;

internal class GetGameQrCodeQueryHandler : IRequestHandler<GetGameQrCodeQuery, GetGameQrCodeQueryModel>
{
    readonly IRepository<GameQrCode> repository;

    public GetGameQrCodeQueryHandler(IRepository<GameQrCode> repository)
    {
        this.repository = repository;
    }

    public async Task<GetGameQrCodeQueryModel> Handle(GetGameQrCodeQuery request, CancellationToken cancellationToken)
    {
        var qrCodes = await this.repository.GetByAsync(x => x.GameId == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(GameQrCode));

        return qrCodes.Adapt<GetGameQrCodeQueryModel>();
    }
}
