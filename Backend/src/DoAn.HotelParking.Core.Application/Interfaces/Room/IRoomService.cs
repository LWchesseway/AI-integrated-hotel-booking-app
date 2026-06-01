using DoAn.HotelParking.Core.Application.DTOs.Room;

namespace DoAn.HotelParking.Core.Application.Interfaces.Room;

public interface IRoomService
{
	Task<IEnumerable<RoomDto>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<(IEnumerable<RoomDto> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default);
	Task<RoomDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
	Task<IEnumerable<RoomDetailDto>> GetByRoomTypeIdAsync(int roomTypeId, CancellationToken cancellationToken = default);
	Task<RoomDto> CreateAsync(CreateRoomDto dto, CancellationToken cancellationToken = default);
	Task<RoomDto?> UpdateAsync(int id, UpdateRoomDto dto, CancellationToken cancellationToken = default);
	Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

	Task<IEnumerable<RoomDetailDto>> GetByHotelIdAsync(int hotelId, CancellationToken cancellationToken = default);
	Task<IEnumerable<RoomDetailDto>> GetAvailableByHotelIdAsync(int hotelId, DateTime checkInDate, DateTime checkOutDate, CancellationToken cancellationToken = default);
	Task<IEnumerable<DateTime>> GetFullyBookedDatesByHotelIdAsync(int hotelId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
	Task<IEnumerable<DateTime>> GetBookedDatesByRoomIdAsync(int roomId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
}
