using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Statistics;
using DH.Domain.Adapters.Statistics.Services;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.SpaceManagement.Commands;

public record AddVirtualParticipantCommand(int SpaceTableId) : IRequest;

internal class AddVirtualParticipantCommandHandler(
    IRepository<SpaceTable> spaceTableRepository,
    IStatisticQueuePublisher statisticQueuePublisher,
    IUserContext userContext) : IRequestHandler<AddVirtualParticipantCommand>
{
    readonly IRepository<SpaceTable> spaceTableRepository = spaceTableRepository;
    readonly IStatisticQueuePublisher statisticQueuePublisher = statisticQueuePublisher;
    readonly IUserContext userContext = userContext;

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

        await this.statisticQueuePublisher.PublishAsync(new StatisticJobQueue.ClubActivityDetectedJob(
            userContext.UserId, DateTime.UtcNow));
    }
}