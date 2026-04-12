using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Domain.Entities.Booking;

namespace DoAn.HotelParking.Core.Application.Interfaces.Booking;

public interface IBookingRepository : IGenericRepository<Domain.Entities.Booking.Booking>
{
    /// <summary>
    /// Lấy danh sách đặt phòng theo khách hàng
    /// </summary>
    Task<IEnumerable<Domain.Entities.Booking.Booking>> GetBookingsByCustomerAsync(int customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy danh sách đặt phòng theo phòng
    /// </summary>
    Task<IEnumerable<Domain.Entities.Booking.Booking>> GetBookingsByRoomAsync(int roomId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy danh sách đặt phòng theo trạng thái
    /// </summary>
    Task<IEnumerable<Domain.Entities.Booking.Booking>> GetBookingsByStatusAsync(int status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy đặt phòng cùng với thông tin chi tiết (phòng, khách hàng, thanh toán)
    /// </summary>
    Task<Domain.Entities.Booking.Booking?> GetBookingDetailAsync(int bookingId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy danh sách đặt phòng chưa được duyệt
    /// </summary>
    Task<IEnumerable<Domain.Entities.Booking.Booking>> GetPendingBookingsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy danh sách đặt phòng hôm nay
    /// </summary>
    Task<IEnumerable<Domain.Entities.Booking.Booking>> GetTodayBookingsAsync(CancellationToken cancellationToken = default);
}
