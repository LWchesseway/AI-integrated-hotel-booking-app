using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Domain.Entities.Hotel;

namespace DoAn.HotelParking.Core.Application.Interfaces.Hotel;

public interface IHotelImageRepository : IGenericRepository<HotelImage>
{
    Task<IEnumerable<HotelImage>> GetByHotelIdAsync(int hotelId, CancellationToken cancellationToken = default);
    Task ClearPrimaryAsync(int hotelId, int? exceptImageId = null, CancellationToken cancellationToken = default);
}
