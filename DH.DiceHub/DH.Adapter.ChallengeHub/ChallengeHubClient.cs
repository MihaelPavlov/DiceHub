 using DH.Domain.Adapters.ChallengeHub;
using Microsoft.AspNetCore.SignalR;

namespace DH.Adapter.ChallengeHub;

public class ChallengeHubClient : Hub, IChallengeHubClient
{
    readonly IHubContext<ChallengeHubClient> _hub;

    public ChallengeHubClient(IHubContext<ChallengeHubClient> hub)
    {
        _hub = hub;
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var userId = httpContext?.Request.Query["userId"].ToString();

        if (!string.IsNullOrEmpty(userId))
        {
            // Add this connection to a group named after the userId
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        }

        await base.OnConnectedAsync();
    }

    public async Task SendChallengeCompleted(string userId, int challengeId)
    {
        // Send to the group corresponding to this user
        await _hub.Clients.Group($"user-{userId}")
            .SendAsync("challengeCompleted", new { ChallengeId = challengeId });
    }

    public async Task SendChallengeUpdated(string userId, int challengeId)
    {
        // Send to the group corresponding to this user
        await _hub.Clients.Group($"user-{userId}")
            .SendAsync("challengeUpdated", new
            {
                ChallengeId = challengeId,
            });
    }
}
