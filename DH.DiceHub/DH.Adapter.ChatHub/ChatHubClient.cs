using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.ChatHub;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Repositories;
using Microsoft.AspNetCore.SignalR;
using System.Net.WebSockets;

namespace DH.Adapter.ChatHub;

public class ChatHubClient : Hub, IChatHubClient
{
    readonly IRepository<Room> roomsRepository;
    readonly IRepository<RoomParticipant> roomParticipantsRepository;
    readonly IRepository<RoomMessages> roomMessagesRepository;
    readonly IUserContext userContext;
    readonly IJwtService jwtService;
    readonly IUserService userService;

    public ChatHubClient(IRepository<Room> roomsRepository, IRepository<RoomParticipant> roomParticipantsRepository,
        IRepository<RoomMessages> roomMessagesRepository, IUserContext userContext, IJwtService jwtService, IUserService userService)
    {
        this.roomsRepository = roomsRepository;
        this.roomParticipantsRepository = roomParticipantsRepository;
        this.roomMessagesRepository = roomMessagesRepository;
        this.userContext = userContext;
        this.jwtService = jwtService;
        this.userService = userService;
    }

    public override Task OnConnectedAsync()
    {
        var accessToken = Context.GetHttpContext().Request.Query["access_token"];

        if (!string.IsNullOrEmpty(accessToken))
        {

            var claimsPrincipal = this.jwtService.ValidateToken(accessToken);

            if (claimsPrincipal != null)
            {
                Context.GetHttpContext().User = claimsPrincipal;
            }
        }
        return base.OnConnectedAsync();
    }

    public async Task AddToGroup(int roomId)
    {
        var room = await this.roomsRepository.GetByAsync(g => g.Id == roomId, CancellationToken.None)
            ?? throw new NotFoundException(nameof(Room), roomId);

        var userGroup = new RoomParticipant { UserId = this.userContext.UserId, Room = room };
        await this.roomParticipantsRepository.AddAsync(userGroup, CancellationToken.None);

        await Groups.AddToGroupAsync(this.Context.ConnectionId, roomId.ToString());
        await Clients.Group(roomId.ToString()).SendAsync("Send", $"{this.Context.ConnectionId} has joined the group {room.Name}.");
    }

    public async Task SendMessageToGroup(int roomId, string message)
    {
        var room = await this.roomsRepository.GetByAsync(g => g.Id == roomId, CancellationToken.None)
            ?? throw new NotFoundException(nameof(Room), roomId);

        var newMessage = new RoomMessages { Timestamp = DateTime.Now, Room = room, MessageContent = message, Sender = this.userContext.UserId };

        var user = await this.userService.GetUserListByIds([this.userContext.UserId], CancellationToken.None);
        await this.roomMessagesRepository.AddAsync(newMessage, CancellationToken.None);
        await Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage", newMessage.Sender, user.First().UserName, newMessage.MessageContent, newMessage.Timestamp);
    }

    public async Task ConnectToGroup(int roomId)
    {
        var room = await this.roomsRepository.GetByAsync(g => g.Id == roomId, CancellationToken.None)
            ?? throw new NotFoundException(nameof(Room), roomId);

        await Groups.AddToGroupAsync(this.Context.ConnectionId, roomId.ToString());
    }
}
