using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.Events.Queries;

public record GetEventImageByIdQuery(int Id) : IRequest<GameImageResult>;

internal class GetEventImageByIdQueryHandler : IRequestHandler<GetEventImageByIdQuery, GameImageResult>
{
    readonly IRepository<EventImage> repository;

    public GetEventImageByIdQueryHandler(IRepository<EventImage> repository)
    {
        this.repository = repository;
    }

    public async Task<GameImageResult> Handle(GetEventImageByIdQuery request, CancellationToken cancellationToken)
    {
        var gameImage = await this.repository.GetByAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(GameImage));

        return new GameImageResult(gameImage.FileName, gameImage.ContentType, gameImage.Data);
    }
}

public record GameImageResult(string FileName, string ContentType, byte[] Data);