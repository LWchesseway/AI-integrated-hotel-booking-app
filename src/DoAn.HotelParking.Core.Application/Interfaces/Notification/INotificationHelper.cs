using DoAn.HotelParking.Core.Domain.Enums;

namespace DoAn.HotelParking.Core.Application.Interfaces.Notification;

public interface INotificationHelper
{
    Task SendBookingCreatedAsync(
        int ownerId,
        int customerId,
        int bookingId,
        CancellationToken cancellationToken = default);

    Task SendBookingStatusChangedAsync(
        int customerId,
        int bookingId,
        BookingStatus newStatus,
        CancellationToken cancellationToken = default);

    Task SendPaymentStatusAsync(
        int customerId,
        int bookingId,
        PaymentStatus status,
        CancellationToken cancellationToken = default);

    Task SendReviewCreatedAsync(
        int ownerId,
        int customerId,
        int reviewId,
        int bookingId,
        byte rating,
        CancellationToken cancellationToken = default);
}
