using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Domain.Entities.Parking;

namespace DoAn.HotelParking.Core.Application.Interfaces.Parking;

public interface IParkingSessionRepository : IGenericRepository<ParkingSession>
{
    /// <summary>
    /// Lấy danh sách phiên đậu xe của người dùng
    /// </summary>
    Task<IEnumerable<ParkingSession>> GetSessionsByUserAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy danh sách phiên đậu xe đang hoạt động
    /// </summary>
    Task<IEnumerable<ParkingSession>> GetActiveSessions(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy danh sách phiên đậu xe theo trạng thái
    /// </summary>
    Task<IEnumerable<ParkingSession>> GetSessionsByStatusAsync(int status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy phiên đậu xe chi tiết cùng với lịch sử biển số
    /// </summary>
    Task<ParkingSession?> GetSessionDetailAsync(int sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy phiên đậu xe đang hoạt động của người dùng
    /// </summary>
    Task<ParkingSession?> GetActiveSessionByUserAsync(int userId, CancellationToken cancellationToken = default);
}
