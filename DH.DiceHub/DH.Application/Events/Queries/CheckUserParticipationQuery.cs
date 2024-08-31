
using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Events.Queries;

public record CheckUserParticipationQuery(int Id) : IRequest<bool>;

internal class CheckUserParticipationQueryHandler : IRequestHandler<CheckUserParticipationQuery, bool>
{
    readonly IRepository<EventParticipant> repository;
    readonly IUserContext userContext;

    public CheckUserParticipationQueryHandler(IRepository<EventParticipant> repository, IUserContext userContext)
    {
        this.repository = repository;
        this.userContext = userContext;
    }

    public async Task<bool> Handle(CheckUserParticipationQuery request, CancellationToken cancellationToken)
    {
        var participant = await this.repository.GetByAsync(x => x.EventId == request.Id && x.UserId == this.userContext.UserId, cancellationToken);
        if (participant is null)
            return false;

        return true;
    }
}
