using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Domain.Entities.Review;

namespace DoAn.HotelParking.Core.Application.Interfaces.Review;

public interface IReviewRepository : IGenericRepository<Domain.Entities.Review.Review>
{
    /// <summary>
    /// Lấy danh sách đánh giá của phòng
    /// </summary>
    Task<IEnumerable<Domain.Entities.Review.Review>> GetReviewsByRoomAsync(int roomId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy danh sách đánh giá của khách hàng
    /// </summary>
    Task<IEnumerable<Domain.Entities.Review.Review>> GetReviewsByCustomerAsync(int customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy danh sách đánh giá theo đặt phòng
    /// </summary>
    Task<IEnumerable<Domain.Entities.Review.Review>> GetReviewsByBookingAsync(int bookingId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy đánh giá chi tiết
    /// </summary>
    Task<Domain.Entities.Review.Review?> GetReviewDetailAsync(int reviewId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kiểm tra khách hàng đã đánh giá phòng này chưa
    /// </summary>
    Task<bool> HasCustomerReviewedRoomAsync(int customerId, int roomId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tính điểm đánh giá trung bình của phòng
    /// </summary>
    Task<double> GetAverageRatingByRoomAsync(int roomId, CancellationToken cancellationToken = default);
}
