using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.SpaceManagement.Commands;

public record RemoveVirtualParticipantCommand(int SpaceTableId, int PartcipantId) : IRequest;

internal class RemoveVirtualParticipantCommandHandler : IRequestHandler<RemoveVirtualParticipantCommand>
{
    readonly IRepository<SpaceTable> spaceTableRepository;
    readonly IRepository<SpaceTableParticipant> spaceTableParticipantRepository;
    readonly IUserContext userContext;

    public RemoveVirtualParticipantCommandHandler(IRepository<SpaceTable> spaceTableRepository, IRepository<SpaceTableParticipant> spaceTableParticipantRepository, IUserContext userContext)
    {
        this.spaceTableRepository = spaceTableRepository;
        this.spaceTableParticipantRepository = spaceTableParticipantRepository;
        this.userContext = userContext;
    }

    public async Task Handle(RemoveVirtualParticipantCommand request, CancellationToken cancellationToken)
    {
        var spaceTable = await this.spaceTableRepository.GetByAsyncWithTracking(x => x.Id == request.SpaceTableId && x.IsTableActive, cancellationToken)
            ?? throw new NotFoundException(nameof(SpaceTable), request.SpaceTableId);

        if (this.userContext.RoleKey != (int)Role.User)
            throw new BadRequestException("Only staff can add virtual users");

        var spaceTableParticipation = await this.spaceTableParticipantRepository.GetByAsyncWithTracking(x => request.PartcipantId == x.Id && spaceTable.Id == x.SpaceTableId, cancellationToken);

        if (spaceTableParticipation == null)
            return;

        await this.spaceTableParticipantRepository.Remove(spaceTableParticipation, cancellationToken);
    }
}