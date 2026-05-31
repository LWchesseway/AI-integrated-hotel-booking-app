using DoAn.HotelParking.Core.Application.DTOs.Hotel;

namespace DoAn.HotelParking.Core.Application.Interfaces.Hotel;

public interface IHotelService
{
	Task<IEnumerable<HotelDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<(IEnumerable<HotelDetailDto> Items, int TotalCount)> GetPagedWithProvinceAsync(
		int pageIndex,
		int pageSize,
		CancellationToken cancellationToken = default);
	Task<(IEnumerable<HotelDetailDto> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default);
	Task<HotelDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
	Task<IEnumerable<HotelDetailDto>> SearchByNameAsync(string hotelName, CancellationToken cancellationToken = default);
	Task<IEnumerable<HotelDetailDto>> GetByProvinceAsync(string province, CancellationToken cancellationToken = default);
	Task<HotelDto> CreateAsync(CreateHotelDto dto, CancellationToken cancellationToken = default);
	Task<HotelDto?> UpdateAsync(int id, UpdateHotelDto dto, CancellationToken cancellationToken = default);
	Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

	Task<HotelDto> CreateOwnedHotelAsync(int ownerId, CreateHotelDto dto, CancellationToken cancellationToken = default);
	Task<HotelDto?> UpdateOwnedHotelAsync(int hotelId, int ownerId, UpdateHotelDto dto, CancellationToken cancellationToken = default);
	Task<HotelDto?> GetByIdWithLocationAsync(int id, CancellationToken cancellationToken = default);
}