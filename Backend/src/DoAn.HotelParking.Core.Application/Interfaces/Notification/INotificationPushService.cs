namespace DoAn.HotelParking.Core.Application.Interfaces.Notification;

public interface INotificationPushService
{
    Task PushToUserAsync(NotificationPushPayload payload, CancellationToken cancellationToken = default);
}

public sealed record NotificationPushPayload(
    int NotificationId,
    int UserId,
    string? Title,
    string? Message,
    byte Type,
    string? RelatedTable,
    int? RelatedId,
    DateTime CreatedAt);
