using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Domain.Entities.Hotel;

namespace DoAn.HotelParking.Core.Application.Interfaces.Hotel;

public interface IHotelRepository : IGenericRepository<DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel>
{
	Task<IEnumerable<DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel>> SearchByNameWithLocationAsync(
		string hotelName,
		CancellationToken cancellationToken = default);

	Task<DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel?> GetByRoomIdAsync(
		int roomId,
		CancellationToken cancellationToken = default);

	Task<IEnumerable<DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel>> GetByProvinceWithLocationAsync(
		string province,
		CancellationToken cancellationToken = default);

	Task<DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel?> GetHotelWithDetailsForRecommendationAsync(
		int hotelId,
		CancellationToken cancellationToken = default);

	Task<IEnumerable<DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel>> GetAllActiveHotelsWithDetailsAsync(
		string? province = null,
		string? ward = null,
		CancellationToken cancellationToken = default);

	Task<(IEnumerable<DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel> Items, int TotalCount)> GetPagedWithProvinceAsync(
		int pageIndex,
		int pageSize,
		CancellationToken cancellationToken = default);

	Task<DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel?> GetByIdWithLocationAsync(
		int id,
		CancellationToken cancellationToken = default);
}
