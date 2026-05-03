using DoAn.HotelParking.Core.Application.DTOs.Notification;
using DoAn.HotelParking.Core.Application.Interfaces.Notification;
using DoAn.HotelParking.Core.Domain.Enums;

namespace DoAn.HotelParking.Core.Application.Services.Notification.Helpers;

public class NotificationHelper : INotificationHelper
{
    private const string BookingsTable = "Bookings";
    private const string ReviewsTable = "Reviews";

    private readonly INotificationService _notificationService;

    public NotificationHelper(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public Task SendBookingCreatedAsync(
        int ownerId,
        int customerId,
        int bookingId,
        CancellationToken cancellationToken = default)
    {
        var title = "Co don dat phong moi";
        var message = $"Khach hang vua dat phong #{bookingId}. Vui long xac nhan.";

        return _notificationService.CreateAsync(new CreateNotificationDto
        {
            UserId = ownerId,
            SenderId = customerId,
            Title = title,
            Message = message,
            Type = (byte)NotificationType.Booking,
            RelatedTable = BookingsTable,
            RelatedId = bookingId
        }, cancellationToken);
    }

    public Task SendBookingStatusChangedAsync(
        int customerId,
        int bookingId,
        BookingStatus newStatus,
        CancellationToken cancellationToken = default)
    {
        if (!TryBuildBookingStatusMessage(newStatus, bookingId, out var title, out var message))
        {
            return Task.CompletedTask;
        }

        return _notificationService.CreateAsync(new CreateNotificationDto
        {
            UserId = customerId,
            SenderId = null,
            Title = title,
            Message = message,
            Type = (byte)NotificationType.Booking,
            RelatedTable = BookingsTable,
            RelatedId = bookingId
        }, cancellationToken);
    }

    public Task SendPaymentStatusAsync(
        int customerId,
        int bookingId,
        PaymentStatus status,
        CancellationToken cancellationToken = default)
    {
        if (!TryBuildPaymentStatusMessage(status, bookingId, out var title, out var message))
        {
            return Task.CompletedTask;
        }

        return _notificationService.CreateAsync(new CreateNotificationDto
        {
            UserId = customerId,
            SenderId = null,
            Title = title,
            Message = message,
            Type = (byte)NotificationType.Payment,
            RelatedTable = BookingsTable,
            RelatedId = bookingId
        }, cancellationToken);
    }

    public Task SendReviewCreatedAsync(
        int ownerId,
        int customerId,
        int reviewId,
        int bookingId,
        byte rating,
        CancellationToken cancellationToken = default)
    {
        var title = "Co danh gia moi";
        var message = $"Khach hang da danh gia booking #{bookingId} ({rating}/5).";

        return _notificationService.CreateAsync(new CreateNotificationDto
        {
            UserId = ownerId,
            SenderId = customerId,
            Title = title,
            Message = message,
            Type = (byte)NotificationType.Review,
            RelatedTable = ReviewsTable,
            RelatedId = reviewId
        }, cancellationToken);
    }

    private static bool TryBuildBookingStatusMessage(
        BookingStatus status,
        int bookingId,
        out string title,
        out string message)
    {
        switch (status)
        {
            case BookingStatus.Confirmed:
                title = "Dat phong da duoc xac nhan";
                message = $"Dat phong #{bookingId} da duoc xac nhan.";
                return true;
            case BookingStatus.Cancelled:
                title = "Dat phong da bi huy";
                message = $"Dat phong #{bookingId} da bi huy.";
                return true;
            case BookingStatus.Completed:
                title = "Dat phong da hoan tat";
                message = $"Dat phong #{bookingId} da hoan tat.";
                return true;
            default:
                title = string.Empty;
                message = string.Empty;
                return false;
        }
    }

    private static bool TryBuildPaymentStatusMessage(
        PaymentStatus status,
        int bookingId,
        out string title,
        out string message)
    {
        switch (status)
        {
            case PaymentStatus.Completed:
                title = "Thanh toan thanh cong";
                message = $"Thanh toan cho booking #{bookingId} da thanh cong.";
                return true;
            case PaymentStatus.Failed:
                title = "Thanh toan that bai";
                message = $"Thanh toan cho booking #{bookingId} that bai. Vui long thu lai.";
                return true;
            default:
                title = string.Empty;
                message = string.Empty;
                return false;
        }
    }
}
