using DoAn.HotelParking.Core.Application.DTOs.RoomType;

namespace DoAn.HotelParking.Core.Application.Interfaces.RoomType;

public interface IRoomTypeService
{
	Task<IEnumerable<RoomTypeDto>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<(IEnumerable<RoomTypeDto> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default);
	Task<RoomTypeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
	Task<IEnumerable<RoomTypeDetailDto>> GetByHotelIdAsync(int hotelId, CancellationToken cancellationToken = default);
	Task<RoomTypeDto> CreateAsync(CreateRoomTypeDto dto, CancellationToken cancellationToken = default);
	Task<RoomTypeDto?> UpdateAsync(int id, UpdateRoomTypeDto dto, CancellationToken cancellationToken = default);
	Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}