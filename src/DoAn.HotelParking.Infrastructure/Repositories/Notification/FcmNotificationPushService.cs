using DoAn.HotelParking.Core.Application.Interfaces.Notification;
using DoAn.HotelParking.Infrastructure.Data;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DoAn.HotelParking.Infrastructure.Repositories.Notification;

public class FcmNotificationPushService : INotificationPushService
{
    private readonly ApplicationDbContext _context;
    private readonly FirebaseApp _firebaseApp;
    private readonly ILogger<FcmNotificationPushService> _logger;

    public FcmNotificationPushService(
        ApplicationDbContext context,
        FirebaseApp firebaseApp,
        ILogger<FcmNotificationPushService> logger)
    {
        _context = context;
        _firebaseApp = firebaseApp;
        _logger = logger;
    }

    public async Task PushToUserAsync(NotificationPushPayload payload, CancellationToken cancellationToken = default)
    {
        var token = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == payload.UserId)
            .Select(u => u.FcmToken)
            .FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogDebug("Skip FCM push because user {UserId} has no token.", payload.UserId);
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();

        var message = new Message
        {
            Token = token,
            Notification = new FirebaseAdmin.Messaging.Notification
            {
                Title = string.IsNullOrWhiteSpace(payload.Title) ? "Hotel Parking" : payload.Title,
                Body = payload.Message ?? string.Empty
            },
            Data = new Dictionary<string, string>
            {
                ["notificationId"] = payload.NotificationId.ToString(),
                ["userId"] = payload.UserId.ToString(),
                ["title"] = payload.Title ?? string.Empty,
                ["message"] = payload.Message ?? string.Empty,
                ["type"] = payload.Type.ToString(),
                ["relatedTable"] = payload.RelatedTable ?? string.Empty,
                ["relatedId"] = payload.RelatedId?.ToString() ?? string.Empty,
                ["createdAt"] = payload.CreatedAt.ToString("O")
            }
        };

        try
        {
            var messageId = await FirebaseMessaging.GetMessaging(_firebaseApp).SendAsync(message);
            _logger.LogInformation(
                "FCM push sent for notification {NotificationId} to user {UserId}. MessageId: {MessageId}",
                payload.NotificationId,
                payload.UserId,
                messageId);
        }
        catch (FirebaseMessagingException ex) when (ShouldClearToken(ex))
        {
            _logger.LogWarning(
                ex,
                "FCM token of user {UserId} is invalid/unregistered. Token will be cleared.",
                payload.UserId);

            await ClearTokenAsync(payload.UserId, cancellationToken);
        }
    }

    private static bool ShouldClearToken(FirebaseMessagingException ex)
    {
        return ex.MessagingErrorCode is MessagingErrorCode.InvalidArgument or MessagingErrorCode.Unregistered;
    }

    private async Task ClearTokenAsync(int userId, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null || string.IsNullOrWhiteSpace(user.FcmToken))
        {
            return;
        }

        user.FcmToken = null;
        user.FcmTokenUpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
