using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.SpaceManagement.Commands;

public record AddVirtualParticipantCommand(int SpaceTableId) : IRequest;

internal class AddVirtualParticipantCommandHandler : IRequestHandler<AddVirtualParticipantCommand>
{
    readonly IRepository<SpaceTable> spaceTableRepository;

    public AddVirtualParticipantCommandHandler(IRepository<SpaceTable> spaceTableRepository)
    {
        this.spaceTableRepository = spaceTableRepository;
    }

    public async Task Handle(AddVirtualParticipantCommand request, CancellationToken cancellationToken)
    {
        var spaceTable = await this.spaceTableRepository.GetByAsyncWithTracking(x => x.Id == request.SpaceTableId, cancellationToken)
            ?? throw new NotFoundException(nameof(SpaceTable), request.SpaceTableId);

        spaceTable.SpaceTableParticipants.Add(new SpaceTableParticipant
        {
            SpaceTableId = spaceTable.Id,
            UserId = "virtual user",
            JoinedAt = DateTime.UtcNow,
            IsVirtualParticipant = true
        });

        await this.spaceTableRepository.SaveChangesAsync(cancellationToken);
    }
}