using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Domain.Entities.Hotel;

namespace DoAn.HotelParking.Core.Application.Interfaces.Rooms;

public interface IRoomRepository : IGenericRepository<Room>
{
    /// <summary>
    /// Lấy danh sách phòng theo khách sạn
    /// </summary>
    Task<IEnumerable<Room>> GetRoomsByHotelAsync(int hotelId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy danh sách phòng theo trạng thái
    /// </summary>
    Task<IEnumerable<Room>> GetRoomsByStatusAsync(int status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy danh sách phòng theo loại phòng
    /// </summary>
    Task<IEnumerable<Room>> GetRoomsByTypeAsync(int roomTypeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy phòng theo số phòng
    /// </summary>
    Task<Room?> GetByRoomNumberAsync(int hotelId, string roomNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy phòng cùng với thông tin chi tiết (ảnh, đặt phòng, đánh giá)
    /// </summary>
    Task<Room?> GetRoomDetailAsync(int roomId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kiểm tra phòng còn trống trong khoảng thời gian
    /// </summary>
    Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy danh sách phòng trống trong khoảng thời gian
    /// </summary>
    Task<IEnumerable<Room>> GetAvailableRoomsAsync(int hotelId, DateTime checkIn, DateTime checkOut, CancellationToken cancellationToken = default);
}
