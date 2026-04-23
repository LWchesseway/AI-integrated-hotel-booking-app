using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Domain.Entities.Hotel;

namespace DoAn.HotelParking.Core.Application.Interfaces.Hotel;

public interface IHotelRepository : IGenericRepository<DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel>
{
	Task<DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel?> GetHotelWithDetailsForRecommendationAsync(
		int hotelId,
		CancellationToken cancellationToken = default);

	Task<IEnumerable<DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel>> GetAllActiveHotelsWithDetailsAsync(
		string? province = null,
		string? ward = null,
		CancellationToken cancellationToken = default);
}
