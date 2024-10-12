using DH.Application.Rewards.Commands;
using DH.Domain.Adapters.Data;
using DH.Domain.Services.Seed;
using MediatR;

namespace DH.Application.Rewards.Seeders;

internal class SystemRewardsSeedService : ISeedService
{
    readonly IMediator mediator;

    public SystemRewardsSeedService(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public async void Seed()
    {
        foreach (var reward in SeedData.REWARD_LIST_DTOS)
        {
            await this.mediator.Send(new CreateSystemRewardCommand(reward, "reward_image.png", "image/png", new MemoryStream()));
        }
    }
}
