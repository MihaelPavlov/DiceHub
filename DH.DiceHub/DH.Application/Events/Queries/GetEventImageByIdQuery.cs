using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.Events.Queries;

public record GetEventImageByIdQuery(int Id) : IRequest<EventImageResult>;

internal class GetEventImageByIdQueryHandler : IRequestHandler<GetEventImageByIdQuery, EventImageResult>
{
    readonly IRepository<EventImage> repository;

    public GetEventImageByIdQueryHandler(IRepository<EventImage> repository)
    {
        this.repository = repository;
    }

    public async Task<EventImageResult> Handle(GetEventImageByIdQuery request, CancellationToken cancellationToken)
    {
        var eventImage = await this.repository.GetByAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(EventImage));

        return new EventImageResult(eventImage.FileName, eventImage.ContentType, eventImage.Data);
    }
}

public record EventImageResult(string FileName, string ContentType, byte[] Data);