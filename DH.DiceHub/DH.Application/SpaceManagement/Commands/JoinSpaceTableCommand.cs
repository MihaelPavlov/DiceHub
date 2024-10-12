using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.SpaceManagement.Commands;

public record JoinSpaceTableCommand(int Id) : IRequest;

internal class JoinSpaceTableCommandHandler : IRequestHandler<JoinSpaceTableCommand>
{
    readonly IRepository<SpaceTable> spaceTableRepository;
    readonly IRepository<SpaceTableParticipant> spaceTableParticipantRepository;
    readonly IUserContext userContext;

    public JoinSpaceTableCommandHandler(IRepository<SpaceTable> spaceTableRepository, IRepository<SpaceTableParticipant> spaceTableParticipantRepository, IUserContext userContext)
    {
        this.spaceTableRepository = spaceTableRepository;
        this.spaceTableParticipantRepository = spaceTableParticipantRepository;
        this.userContext = userContext;
    }

    public async Task Handle(JoinSpaceTableCommand request, CancellationToken cancellationToken)
    {
        var spaceTable = await this.spaceTableRepository.GetByAsyncWithTracking(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(SpaceTable), request.Id);

        var spaceTableParticipations = spaceTable.SpaceTableParticipants.Where(x => x.SpaceTableId == spaceTable.Id).ToList();

        if (spaceTableParticipations.Count >= spaceTable.MaxPeople)
            throw new ValidationErrorsException("MaxPeople", "Max People was reached for this room");

        spaceTable.SpaceTableParticipants.Add(new SpaceTableParticipant
        {
            SpaceTableId = spaceTable.Id,
            UserId = this.userContext.UserId,
        });

        await this.spaceTableRepository.SaveChangesAsync(cancellationToken);
    }
}