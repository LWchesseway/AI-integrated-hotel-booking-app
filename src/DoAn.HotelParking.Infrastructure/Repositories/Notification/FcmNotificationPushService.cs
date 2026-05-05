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
        var tokens = await _context.FcmTokens
            .AsNoTracking()
            .Where(t => t.UserId == payload.UserId)
            .Select(t => t.Token)
            .ToListAsync(cancellationToken);

        if (tokens.Count == 0)
        {
            _logger.LogDebug("Skip FCM push because user {UserId} has no token.", payload.UserId);
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();

        foreach (var token in tokens)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                continue;
            }

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
            catch (FirebaseMessagingException ex) when (ShouldRemoveToken(ex))
            {
                _logger.LogWarning(
                    ex,
                    "FCM token of user {UserId} is invalid/unregistered. Token will be removed.",
                    payload.UserId);

                await RemoveTokenAsync(token, cancellationToken);
            }
        }
    }

    private static bool ShouldRemoveToken(FirebaseMessagingException ex)
    {
        return ex.MessagingErrorCode is MessagingErrorCode.InvalidArgument or MessagingErrorCode.Unregistered;
    }

    private async Task RemoveTokenAsync(string token, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return;
        }

        var entity = await _context.FcmTokens.FirstOrDefaultAsync(t => t.Token == token, cancellationToken);
        if (entity is null)
        {
            return;
        }

        _context.FcmTokens.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
