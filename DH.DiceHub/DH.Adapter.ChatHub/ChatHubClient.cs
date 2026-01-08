using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.ChatHub;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using Microsoft.AspNetCore.SignalR;
using System.Net.WebSockets;

namespace DH.Adapter.ChatHub;

public class ChatHubClient : Hub, IChatHubClient
{
    readonly IRepository<Room> roomsRepository;
    readonly IRepository<RoomParticipant> roomParticipantsRepository;
    readonly IRepository<RoomMessage> roomMessagesRepository;
    readonly IUserContext userContext;
    readonly ITokenService jwtService;
    readonly IUserManagementService userManagementService;

    public ChatHubClient(
        IRepository<Room> roomsRepository, 
        IRepository<RoomParticipant> roomParticipantsRepository,
        IRepository<RoomMessage> roomMessagesRepository, 
        IUserContext userContext,
        ITokenService jwtService,
        IUserManagementService userManagementService)
    {
        this.roomsRepository = roomsRepository;
        this.roomParticipantsRepository = roomParticipantsRepository;
        this.roomMessagesRepository = roomMessagesRepository;
        this.userContext = userContext;
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
        var room = await this.roomsRepository.GetByAsync(g => g.Id == roomId, CancellationToken.None)
            ?? throw new NotFoundException(nameof(Room), roomId);

        var newMessage = new RoomMessage { CreatedDate = DateTime.UtcNow, RoomId = room.Id, MessageContent = message, Sender = this.userContext.UserId! };

        var user = await this.userManagementService.GetUserListByIds([this.userContext.UserId!], CancellationToken.None);
        await this.roomMessagesRepository.AddAsync(newMessage, CancellationToken.None);
        await Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage", newMessage.Sender, user.First().UserName, newMessage.MessageContent, newMessage.CreatedDate);
    }

    public async Task ConnectToGroup(int roomId)
    {
        var room = await this.roomsRepository.GetByAsync(g => g.Id == roomId, CancellationToken.None)
            ?? throw new NotFoundException(nameof(Room), roomId);

        await Groups.AddToGroupAsync(this.Context.ConnectionId, roomId.ToString());
    }
}
