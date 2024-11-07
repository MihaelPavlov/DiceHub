using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Adapters.PushNotifications.Messages.Models;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DH.Adapter.PushNotifications;

internal class PushNotificationsService : IPushNotificationsService
{
    readonly ILogger<PushNotificationsService> logger;
    readonly IRepository<UserNotification> userNotificationRepository;
    readonly IRepository<UserDeviceToken> deviceTokenRepository;
    readonly IUserContext userContext;

    public PushNotificationsService(ILogger<PushNotificationsService> logger, IRepository<UserNotification> userNotificationRepository, IRepository<UserDeviceToken> deviceTokenRepository, IUserContext userContext)
    {
        this.logger = logger;
        this.userNotificationRepository = userNotificationRepository;
        this.deviceTokenRepository = deviceTokenRepository;
        this.userContext = userContext;
    }

    public async Task<bool> AreAnyActiveNotifcations()
    {
        var result = await this.userNotificationRepository.GetWithPropertiesAsync(x => x.UserId == this.userContext.UserId, x => new GetUserNotificationsModel
        {
            HasBeenViewed = x.HasBeenViewed,
        }, CancellationToken.None);

        return result.Any(x => !x.HasBeenViewed);
    }

    public async Task<IEnumerable<GetUserNotificationsModel>> GetNotificationsByUserId()
    {
        var result = await this.userNotificationRepository.GetWithPropertiesAsync(x => x.UserId == this.userContext.UserId, x => new GetUserNotificationsModel
        {
            Id = x.Id,
            MessageBody = x.MessageBody,
            MessageId = x.MessageId,
            MessageTitle = x.MessageTitle,
            MessageType = x.MessageType,
            HasBeenViewed = x.HasBeenViewed,
            CreatedDate = x.CreatedDate
        }, CancellationToken.None);

        return result.OrderByDescending(x => x.CreatedDate);
    }

    public async Task MarkedNotificationAsViewed(int notificationId)
    {
        var notification = await this.userNotificationRepository.GetByAsyncWithTracking(x => x.Id == notificationId && this.userContext.UserId == x.UserId, CancellationToken.None);
        if (notification != null)
        {
            notification.HasBeenViewed = true;
            await this.userNotificationRepository.SaveChangesAsync(CancellationToken.None);
        }
    }

    public async Task SendMessageAsync(MessageRequest message)
    {
        var deviceToken = await this.deviceTokenRepository.GetByAsync(x => x.DeviceToken == message.DeviceToken && this.userContext.UserId == x.UserId, CancellationToken.None);

        if (deviceToken is null)
        {
            this.logger.LogWarning("Message from type {typeMessage}, was not send", typeof(MessageRequest));
            return;
        }

        try
        {
            var responseId = await FirebaseMessaging.DefaultInstance.SendAsync(new Message
            {
                Token = message.DeviceToken,
                Notification = new Notification
                {
                    Title = message.Title,
                    Body = message.Body
                }
            });

            if (string.IsNullOrEmpty(responseId))
            {
                this.logger.LogWarning("Message from type {typeMessage}, was not send", typeof(MessageRequest));
                return;
            }

            await this.userNotificationRepository.AddAsync(new UserNotification
            {
                UserId = this.userContext.UserId,
                MessageBody = message.Body,
                MessageTitle = message.Title,
                MessageId = responseId.Split("/").Last(),
                MessageType = message.GetType().ToString().Split(".").Last(),
                CreatedDate = DateTime.UtcNow,
            }, CancellationToken.None);
        }
        catch (Exception ex)
        {
            this.logger.LogError("Message error was catched exception -> {exception}", JsonSerializer.Serialize(ex));
        }
    }

    public async Task SendMultipleMessagesAsync(MultipleMessageRequest message)
    {
        var deviceTokens = await this.deviceTokenRepository.GetWithPropertiesAsync(x => message.Tokens.Contains(x.DeviceToken), x => x, CancellationToken.None);

        var notificationMessage = new MulticastMessage
        {
            Tokens = message.Tokens,
            Notification = new Notification
            {
                Title = message.Title,
                Body = message.Body
            }
        };

        var result = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(notificationMessage);

        for (int i = 0; i < result.Responses.Count; i++)
        {
            var response = result.Responses[i];
            var token = notificationMessage.Tokens[i];

            var deviceToken = deviceTokens.FirstOrDefault(dt => dt.DeviceToken == token);

            if (deviceToken != null && response.IsSuccess)
            {
                await this.userNotificationRepository.AddAsync(new UserNotification
                {
                    UserId = deviceToken.UserId,
                    MessageBody = message.Body,
                    MessageTitle = message.Title,
                    MessageId = response.MessageId,
                    MessageType = message.GetType().ToString().Split(".").Last(),
                    CreatedDate = DateTime.UtcNow,
                }, CancellationToken.None);
            }
        }

        this.logger.LogWarning("Sent message from type {TypeOfMessage}: {SuccessCount} successful, {FailureCount} failed.", typeof(MessageRequest), result.SuccessCount, result.FailureCount);
    }
}
