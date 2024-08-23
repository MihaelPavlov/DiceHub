using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Games.Queries.Games;

public record GetGameImageByIdQuery(int Id) : IRequest<GameImageResult>;

internal class GetGameImageByIdQueryHandler : IRequestHandler<GetGameImageByIdQuery, GameImageResult>
{
    readonly IRepository<GameImage> repository;

    public GetGameImageByIdQueryHandler(IRepository<GameImage> repository)
    {
        this.repository = repository;
    }
    public async Task<GameImageResult> Handle(GetGameImageByIdQuery request, CancellationToken cancellationToken)
    {
        var gameImage = await this.repository.GetByAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(GameImage));

        return new GameImageResult(gameImage.FileName, gameImage.ContentType, gameImage.Data);
    }
}

public record GameImageResult(string FileName, string ContentType, byte[] Data);