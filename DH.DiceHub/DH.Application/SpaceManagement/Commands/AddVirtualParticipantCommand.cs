using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Localization;
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
    IRepository<SpaceTableParticipant> tableParticipantsepository,
    IStatisticQueuePublisher statisticQueuePublisher,
    IUserContext userContext,
    ILocalizationService localizer) : IRequestHandler<AddVirtualParticipantCommand>
{
    readonly IRepository<SpaceTable> spaceTableRepository = spaceTableRepository;
    readonly IRepository<SpaceTableParticipant> tableParticipantsepository = tableParticipantsepository;
    readonly IStatisticQueuePublisher statisticQueuePublisher = statisticQueuePublisher;
    readonly IUserContext userContext = userContext;
    readonly ILocalizationService localizer = localizer;

    public async Task Handle(AddVirtualParticipantCommand request, CancellationToken cancellationToken)
    {
        var spaceTable = await this.spaceTableRepository.GetByAsyncWithTracking(x => x.Id == request.SpaceTableId, cancellationToken)
            ?? throw new NotFoundException(nameof(SpaceTable), request.SpaceTableId);

        var participants = await this.tableParticipantsepository.GetWithPropertiesAsync(x => x.SpaceTableId == request.SpaceTableId, x => x.Id, cancellationToken);

        if (participants.Count >= spaceTable.MaxPeople)
            throw new ValidationErrorsException("MaxPeople", this.localizer["RoomMaxPeople"]);

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