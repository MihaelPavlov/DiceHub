using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.ChatHub;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.Domain.Services;
using DH.OperationResultCore.Exceptions;
using Microsoft.AspNetCore.SignalR;
using System.Net.WebSockets;
using System.Security.Claims;

namespace DH.Adapter.ChatHub;

public class ChatHubClient : Hub, IChatHubClient
{
    readonly IRepository<Room> roomsRepository;
    readonly IRepository<RoomParticipant> roomParticipantsRepository;
    readonly IRepository<RoomMessage> roomMessagesRepository;
    readonly ITenantContextScopeRunner tenantContextScopeRunner;
    readonly ITokenService jwtService;
    readonly IUserManagementService userManagementService;

    public ChatHubClient(
        IRepository<Room> roomsRepository,
        IRepository<RoomParticipant> roomParticipantsRepository,
        IRepository<RoomMessage> roomMessagesRepository,
        ITenantContextScopeRunner tenantContextScopeRunner,
        ITokenService jwtService,
        IUserManagementService userManagementService)
    {
        this.roomsRepository = roomsRepository;
        this.roomParticipantsRepository = roomParticipantsRepository;
        this.roomMessagesRepository = roomMessagesRepository;
        this.tenantContextScopeRunner = tenantContextScopeRunner;
        this.jwtService = jwtService;
        this.userManagementService = userManagementService;
    }

    public override Task OnConnectedAsync()
    {
        var accessToken = Context.GetHttpContext().Request.Query["access_token"];

        if (!string.IsNullOrEmpty(accessToken))
        {
            var claimsPrincipal = this.jwtService.ValidateToken(accessToken);

            if (claimsPrincipal != null)
            {
                Context.GetHttpContext().Request.Headers.Add("Authorization", $"Bearer {accessToken}");
                Context.GetHttpContext().User = claimsPrincipal;
            }
        }
        return base.OnConnectedAsync();
    }

    public async Task SendMessageToGroup(int roomId, string message)
    {
        var (tenantId, userId) = this.GetTenantAndUserId();

        await this.tenantContextScopeRunner.RunAsTenantAsync(tenantId, async () =>
        {
            var room = await this.roomsRepository.GetByAsync(g => g.Id == roomId, CancellationToken.None)
                ?? throw new NotFoundException(nameof(Room), roomId);

            var newMessage = new RoomMessage { CreatedDate = DateTime.UtcNow, RoomId = room.Id, MessageContent = message, Sender = userId };

            var user = await this.userManagementService.GetUserListByIds([userId], CancellationToken.None);
            await this.roomMessagesRepository.AddAsync(newMessage, CancellationToken.None);
            await Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage", newMessage.Sender, user.First().UserName, newMessage.MessageContent, newMessage.CreatedDate);
        });
    }

    public async Task ConnectToGroup(int roomId)
    {
        var (tenantId, _) = this.GetTenantAndUserId();

        await this.tenantContextScopeRunner.RunAsTenantAsync(tenantId, async () =>
        {
            var room = await this.roomsRepository.GetByAsync(g => g.Id == roomId, CancellationToken.None)
                ?? throw new NotFoundException(nameof(Room), roomId);

            await Groups.AddToGroupAsync(this.Context.ConnectionId, roomId.ToString());
        });
    }

    /// <summary>
    /// SignalR creates a fresh Hub instance (and DI scope) per method invocation, and the
    /// ambient IHttpContextAccessor is not reliably populated for invocations after the
    /// initial connect request - so tenant-scoped repository access here must go through
    /// ITenantContextScopeRunner (the same mechanism background jobs use) rather than
    /// leaning on the ambient accessor. Context.GetHttpContext() IS reliable - it always
    /// returns this connection's own HttpContext, kept up to date by OnConnectedAsync
    /// above - so the tenant/user claims are read directly from it.
    /// </summary>
    private (string tenantId, string userId) GetTenantAndUserId()
    {
        var user = this.Context.GetHttpContext()?.User;
        var tenantId = user?.FindFirst("tenant_id")?.Value;
        var userId = user?.FindFirst(ClaimTypes.Sid)?.Value;

        if (string.IsNullOrWhiteSpace(tenantId) || string.IsNullOrWhiteSpace(userId))
            throw new UnauthorizedAccessException("Chat hub connection is missing tenant/user claims.");

        return (tenantId, userId);
    }
}
