using DH.Application.Challenges.Commands;
using DH.Domain.Adapters.Data;
using DH.Domain.Services.Seed;
using MediatR;

namespace DH.Application.Challenges.Seeders;

internal class ChallengesSeedService : ISeedService
{
    readonly IMediator mediator;

    public ChallengesSeedService(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public async void Seed()
    {
        foreach (var challenge in SeedData.CHALLENGE_LIST_DTOS)
        {
            await this.mediator.Send(new CreateChallengeCommand(challenge));
        }
    }
}
