using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Services;
using DH.Messaging.Publisher;
using DH.ServiceBusWorker;
using MediatR;

namespace DH.Application.Games.Queries.Games;

public record GetGameByIdQuery(int Id) : IRequest<GetSystemRewardByIdQueryModel>;

internal class GetGameByIdQueryHandler : IRequestHandler<GetGameByIdQuery, GetSystemRewardByIdQueryModel>
{
    readonly IGameService gameService;
    readonly IUserContext userContext;
    readonly IRabbitMqClient _rabbitMqClient;

    public GetGameByIdQueryHandler(IGameService gameService, IUserContext userContext, IRabbitMqClient rabbitMqClient)
    {
        this.gameService = gameService;
        this.userContext = userContext;
        this._rabbitMqClient = rabbitMqClient;
    }

    public async Task<GetSystemRewardByIdQueryModel> Handle(GetGameByIdQuery request, CancellationToken cancellationToken)
    {
        var message = new EventMessage<ParticipationAgreementActivatedMessage>
        {
            MessageId = Guid.NewGuid().ToString(),
            DateTime = DateTimeOffset.UtcNow,
            Body = new ParticipationAgreementActivatedMessage()
        };

        await _rabbitMqClient.Publish("my_exchange", "participation.agreement.activated", message);

        var message1 = new EventMessage<PartProcessMessage>
        {
            MessageId = Guid.NewGuid().ToString(),
            DateTime = DateTimeOffset.UtcNow,
            Body = new PartProcessMessage
            {
                Name = "Test",
            }
        };

        await _rabbitMqClient.Publish("my_exchange", "participation.agreement.process", message1);


        return await this.gameService.GetGameByIdAsync(request.Id, userContext.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Game), request.Id);
    }
}
