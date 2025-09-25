using DH.Domain.Adapters.ChallengeHub;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace DH.Adapter.ChallengeHub;

public class ChallengeHubClient : Hub, IChallengeHubClient
{
    static readonly ConcurrentDictionary<string, HashSet<string>> UserConnections = new();

    readonly IHubContext<ChallengeHubClient> hub;
    readonly IPushNotificationsService pushNotificationsService;

    public ChallengeHubClient(IHubContext<ChallengeHubClient> hub, IPushNotificationsService pushNotificationsService)
    {
        this.hub = hub;
        this.pushNotificationsService = pushNotificationsService;
    }

    public override Task OnConnectedAsync()
    {
        var userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
        if (!string.IsNullOrEmpty(userId))
        {
            UserConnections.TryAdd(userId, new HashSet<string>());
            lock (UserConnections[userId])
            {
                UserConnections[userId].Add(Context.ConnectionId);
            }

            Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        }

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        var userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
        if (!string.IsNullOrEmpty(userId) && UserConnections.TryGetValue(userId, out var connections))
        {
            lock (connections)
            {
                connections.Remove(Context.ConnectionId);
                if (connections.Count == 0)
                {
                    UserConnections.TryRemove(userId, out _);
                }
            }
        }

        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendChallengeCompleted(string userId, string challengeGameName, int rewardPoints)
    {
        if (IsUserConnected(userId))
        {
            await this.hub.Clients.Group($"user-{userId}")
                .SendAsync("challengeCompleted", new { challengeGameName, rewardPoints });
        }
        else
        {
            await this.pushNotificationsService.SendNotificationToUsersAsync(
                [userId],
                new ChallengeCompletedNotification
                {
                    ChallengeName = challengeGameName,
                    RewardPoints = rewardPoints,
                }, CancellationToken.None);
        }
    }

    public async Task SendChallengeUpdated(string userId, string challengeGameName)
    {
        if (IsUserConnected(userId))
        {
            // Send to the group corresponding to this user
            await this.hub.Clients.Group($"user-{userId}")
                .SendAsync("challengeUpdated", new { challengeGameName });
        }
        else
        {
            await this.pushNotificationsService.SendNotificationToUsersAsync(
                [userId],
                new ChallengeUpdatedNotification
                {
                    ChallengeName = challengeGameName,
                }, CancellationToken.None);
        }
    }

    private bool IsUserConnected(string userId)
    {
        return UserConnections.ContainsKey(userId);
    }
}
