using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Domain.Entities.Hotel;

namespace DoAn.HotelParking.Core.Application.Interfaces.RoomType;

public interface IRoomTypeRepository : IGenericRepository<DoAn.HotelParking.Core.Domain.Entities.Hotel.RoomType>
{
	Task<IEnumerable<DoAn.HotelParking.Core.Domain.Entities.Hotel.RoomType>> GetByHotelIdWithRoomsAsync(
		int hotelId,
		CancellationToken cancellationToken = default);
}
