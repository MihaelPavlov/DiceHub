using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Adapters.PushNotifications.Messages.Common;
using System.Text.Json;

namespace DH.Domain.Adapters.PushNotifications;

public static class NotificationTypeRegistry
{
    private static readonly Dictionary<string, Type> Types = new()
    {
        { nameof(EventDeletedNotification), typeof(EventDeletedNotification) },
        { nameof(EventReminderNotification), typeof(EventReminderNotification) },
        { nameof(EventStartDateUpdatedNotification), typeof(EventStartDateUpdatedNotification) },
        { nameof(GameReservationApprovedNotification), typeof(GameReservationApprovedNotification) },
        { nameof(GameReservationDeclinedNotification), typeof(GameReservationDeclinedNotification) },
        { nameof(GameReservationReminderNotification), typeof(GameReservationReminderNotification) },
        { nameof(GameReservationPublicNoteUpdatedNotification), typeof(GameReservationPublicNoteUpdatedNotification) },
        { nameof(NewEventAddedNotification), typeof(NewEventAddedNotification) },
        { nameof(RegistrationNotification), typeof(RegistrationNotification) },
        { nameof(RewardExpirationReminderNotification), typeof(RewardExpirationReminderNotification) },
        { nameof(RewardExpiredNotification), typeof(RewardExpiredNotification) },
        { nameof(SpaceTableApprovedNotification), typeof(SpaceTableApprovedNotification) },
        { nameof(SpaceTableDeclinedNotification), typeof(SpaceTableDeclinedNotification) },
        { nameof(SpaceTablePublicNoteUpdatedNotification), typeof(SpaceTablePublicNoteUpdatedNotification) },
        { nameof(SpaceTableReservationManagementNotification), typeof(SpaceTableReservationManagementNotification) },
        { nameof(RoomGameChangedNotification), typeof(RoomGameChangedNotification) },
        { nameof(RoomStartDateChangedNotification), typeof(RoomStartDateChangedNotification) },
        { nameof(RoomParticipantJoinedNotification), typeof(RoomParticipantJoinedNotification) },
        { nameof(ChallengeCompletedNotification), typeof(ChallengeCompletedNotification) },
        { nameof(ChallengeUpdatedNotification), typeof(ChallengeUpdatedNotification) },
        { nameof(RewardGrantedNotification), typeof(RewardGrantedNotification) },
    };

    public static bool TryGetType(string messageType, out Type type) => Types.TryGetValue(messageType, out type!);

    public static RenderableNotification? DeserializeNotification(string messageType, string payloadJson)
    {
        if (!TryGetType(messageType, out var type))
            return null;

        return (RenderableNotification?)JsonSerializer.Deserialize(payloadJson, type);
    }
}
