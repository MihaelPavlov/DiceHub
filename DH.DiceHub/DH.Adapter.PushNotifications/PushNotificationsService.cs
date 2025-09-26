using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Localization;
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
    readonly INotificationRenderer notificationRenderer;
    readonly ILocalizationService localizer;

    public PushNotificationsService(
        ILogger<PushNotificationsService> logger, IRepository<UserNotification> userNotificationRepository,
        IRepository<UserDeviceToken> deviceTokenRepository, IUserContext userContext,
        INotificationRenderer notificationRenderer, ILocalizationService localizer)
    {
        this.logger = logger;
        this.userNotificationRepository = userNotificationRepository;
        this.deviceTokenRepository = deviceTokenRepository;
        this.userContext = userContext;
        this.notificationRenderer = notificationRenderer;
        this.localizer = localizer;
    }

    public async Task<bool> AreAnyActiveNotifcations(CancellationToken cancellationToken)
    {
        var result = await this.userNotificationRepository.GetWithPropertiesAsync(x => x.UserId == this.userContext.UserId, x => new GetUserNotificationsModel
        {
            HasBeenViewed = x.HasBeenViewed,
        }, cancellationToken);

        return result.Any(x => !x.HasBeenViewed);
    }

    public async Task<IEnumerable<GetUserNotificationsModel>> GetNotificationsByUserId(int page, int pageSize, CancellationToken cancellationToken)
    {
        var skip = (page - 1) * pageSize;

        var notifications = await this.userNotificationRepository
            .GetWithPropertiesAsync(x => x.UserId == this.userContext.UserId, x => x, cancellationToken);

        var pagedNotifications = notifications
            .OrderByDescending(x => x.CreatedDate)
            .Skip(skip)
            .Take(pageSize)
            .ToList(); 

        var result = new List<GetUserNotificationsModel>();

        foreach (var n in pagedNotifications)
        {
            var payload = NotificationTypeRegistry.DeserializeNotification(n.MessageType, n.PayloadJson);
            if (payload == null)
            {
                this.logger.LogWarning("Unregistered notification type - {MessageType}", n.MessageType);
                continue;
            }

            var notificationRenderResult = await this.notificationRenderer.RenderMessageBody(payload, n.UserId);
            if (notificationRenderResult == null)
                continue;

            result.Add(new GetUserNotificationsModel
            {
                Id = n.Id,
                MessageId = n.MessageId,
                MessageType = n.MessageType,
                MessageTitle = notificationRenderResult.Title,
                MessageBody = notificationRenderResult.Body,
                HasBeenViewed = n.HasBeenViewed,
                CreatedDate = n.CreatedDate
            });
        }

        return result;
    }

    public async Task MarkedNotificationAsViewed(int notificationId, CancellationToken cancellationToken)
    {
        var notification = await this.userNotificationRepository.GetByAsyncWithTracking(x => x.Id == notificationId && this.userContext.UserId == x.UserId, CancellationToken.None);
        if (notification != null)
        {
            notification.HasBeenViewed = true;
            await this.userNotificationRepository.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task RefreshUserDeviceToken(string deviceToken, CancellationToken cancellationToken)
    {
        var userDeviceToken = await this.deviceTokenRepository.GetByAsyncWithTracking(x => x.UserId == this.userContext.UserId, cancellationToken);

        if (userDeviceToken != null)
        {
            userDeviceToken.DeviceToken = deviceToken;
            await this.deviceTokenRepository.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task SendUserNotificationAsync(RenderableNotification message)
    {
        var deviceToken = await this.deviceTokenRepository.GetByAsync(x => x.DeviceToken == message.DeviceToken && this.userContext.UserId == x.UserId, CancellationToken.None);

        if (deviceToken is null)
        {
            this.logger.LogWarning("Message from type {typeMessage}, was not send", typeof(RenderableNotification));
            return;
        }

        try
        {
            //var responseId = await FirebaseMessaging.DefaultInstance.SendAsync(new Message
            //{
            //    Token = message.DeviceToken,
            //    //Notification = new Notification
            //    //{
            //    //    Title = message.Title,
            //    //    Body = message.Body,
            //    //},
            //    //Android = new AndroidConfig
            //    //{
            //    //    Notification = new AndroidNotification
            //    //    {
            //    //        Title = message.Title,
            //    //        Body = message.Body,
            //    //        ImageUrl = "https://dicehub.online/shared/assets/images/dicehub_favicon_2.png", // ✅ icon shown in web push
            //    //        ClickAction = "https://dicehub.online/login"
            //    //    }
            //    //},
            //    Data = new Dictionary<string, string>
            //    {
            //        { "title", message.Title },
            //        { "body", message.Body },
            //        { "icon", "https://dicehub.online/shared/assets/images/dicehub_favicon_2.png" },
            //        { "click_action", "https://dicehub.online/login" },
            //    },

            //    Webpush = new WebpushConfig
            //    {
            //        Headers = new Dictionary<string, string>
            //        {
            //            { "Urgency", "high" }
            //        }
            //    }
            //});

            //if (string.IsNullOrEmpty(responseId))
            //{
            //    this.logger.LogWarning("Message from type {typeMessage}, was not send", typeof(RenderableNotification));
            //    return;
            //}

            //await this.userNotificationRepository.AddAsync(new UserNotification
            //{
            //    UserId = this.userContext.UserId,
            //    MessageBody = message.Body,
            //    MessageTitle = message.Title,
            //    MessageId = responseId.Split("/").Last(),
            //    MessageType = message.GetType().ToString().Split(".").Last(),
            //    CreatedDate = DateTime.UtcNow,
            //}, CancellationToken.None);
        }
        catch (Exception ex)
        {
            this.logger.LogError("Message error was catched exception -> {exception}", ex.Message);
        }
    }

    public async Task SendBulkNotificationsAsync(MultipleMessageRequest message)
    {
        var deviceTokens = await this.deviceTokenRepository.GetWithPropertiesAsync(x => message.Tokens.Contains(x.DeviceToken), x => x, CancellationToken.None);

        var notificationMessage = new MulticastMessage
        {
            Tokens = message.Tokens,
            //Notification = new Notification
            //{
            //    Title = message.Title,
            //    Body = message.Body
            //},
            //Android = new AndroidConfig
            //{
            //    Notification = new AndroidNotification
            //    {
            //        Title = message.Title,
            //        Body = message.Body,
            //        ImageUrl = "https://dicehub.online/shared/assets/images/dicehub_favicon_2.png", // ✅ icon shown in web push
            //        ClickAction = "https://dicehub.online/login"
            //    }
            //},
            Data = new Dictionary<string, string>
                {
                    { "title", message.Title },
                    { "body", message.Body },
                    { "icon", "https://dicehub.online/shared/assets/images/dicehub_favicon_2.png" },
                    { "click_action", "https://dicehub.online/login" }
                },

            Webpush = new WebpushConfig
            {
                Headers = new Dictionary<string, string>
                    {
                        { "Urgency", "high" }
                    }
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
                //await this.userNotificationRepository.AddAsync(new UserNotification
                //{
                //    UserId = deviceToken.UserId,
                //    MessageBody = message.Body,
                //    MessageTitle = message.Title,
                //    MessageId = response.MessageId,
                //    MessageType = message.GetType().ToString().Split(".").Last(),
                //    CreatedDate = DateTime.UtcNow,
                //}, CancellationToken.None);
            }
        }

        this.logger.LogWarning("Sent message from type {TypeOfMessage}: {SuccessCount} successful, {FailureCount} failed.", typeof(RenderableNotification), result.SuccessCount, result.FailureCount);
    }

    public async Task SendNotificationToUsersAsync<TPayload>(
        List<string> userIds, TPayload payload, CancellationToken cancellationToken)
            where TPayload : RenderableNotification
    {
        var deviceTokens = await this.deviceTokenRepository.GetWithPropertiesAsync(x => userIds.Contains(x.UserId), x => x, CancellationToken.None);
        foreach (var userId in userIds)
        {
            var deviceToken = deviceTokens.FirstOrDefault(x => x.UserId == userId);
            if (deviceToken is null)
            {
                this.logger.LogWarning("Message from type {typeMessage}, was not send", typeof(RenderableNotification));
                continue;
            }
            try
            {
                // Serialize the payload to JSON to store in DB
                var payloadJson = JsonSerializer.Serialize(payload);

                var notification = new UserNotification
                {
                    UserId = userId,
                    PayloadJson = payloadJson,
                    MessageType = typeof(TPayload).Name,
                    CreatedDate = DateTime.UtcNow
                };
                await this.userNotificationRepository.AddAsync(notification, CancellationToken.None);

                var notificationPayload = await notificationRenderer.RenderMessageBody(payload, userId);
                if (notificationPayload == null)
                {
                    this.logger.LogError("");
                    continue;
                }

                var responseId = await FirebaseMessaging.DefaultInstance.SendAsync(new Message
                {
                    Token = deviceToken.DeviceToken,
                    //Notification = new Notification
                    //{
                    //    Title = message.Title,
                    //    Body = message.Body
                    //},
                    //Android = new AndroidConfig
                    //{
                    //    Notification = new AndroidNotification
                    //    {
                    //        Title = message.Title,
                    //        Body = message.Body,
                    //        ImageUrl = "https://dicehub.online/shared/assets/images/dicehub_favicon_2.png", // ✅ icon shown in web push
                    //        ClickAction = "https://dicehub.online/login"
                    //    }
                    //},
                    Data = new Dictionary<string, string>
                    {
                        { "title", notificationPayload.Title },
                        { "body", notificationPayload.Body },
                        { "icon", "https://dicehub.online/shared/assets/images/dicehub_favicon_2.png" },
                        { "click_action", "https://dicehub.online/login" }
                    },
                    Webpush = new WebpushConfig
                    {
                        Headers = new Dictionary<string, string>
                        {
                            { "Urgency", "high" }
                        }
                    }
                });

                if (string.IsNullOrEmpty(responseId))
                {
                    this.logger.LogWarning("Message from type {typeMessage}, was not send", typeof(RenderableNotification));
                    continue;
                }

                notification.MessageId = responseId.Split("/").Last();
                await this.userNotificationRepository.Update(notification, CancellationToken.None);
            }
            catch (Exception ex)
            {
                this.logger.LogError("Message error was catched exception -> {exception}", ex.Message);
            }
        }
    }

    public async Task ClearUserAllNotifications(CancellationToken cancellationToken)
    {
        var userNotifications = await this.userNotificationRepository.GetWithPropertiesAsTrackingAsync(x => x.UserId == this.userContext.UserId, x => x, cancellationToken);

        await this.userNotificationRepository.RemoveRange(userNotifications, cancellationToken);
    }

    public async Task MarkedAsViewAllUserNotifications(CancellationToken cancellationToken)
    {
        var userNotifications = await this.userNotificationRepository.GetWithPropertiesAsTrackingAsync(x => x.UserId == this.userContext.UserId, x => x, cancellationToken);

        foreach (var item in userNotifications)
        {
            item.HasBeenViewed = true;
        }

        await this.userNotificationRepository.SaveChangesAsync(cancellationToken);
    }
}
