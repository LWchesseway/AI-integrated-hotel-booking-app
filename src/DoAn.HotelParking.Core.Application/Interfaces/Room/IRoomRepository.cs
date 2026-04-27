using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Domain.Entities.Hotel;

namespace DoAn.HotelParking.Core.Application.Interfaces.Room;

public interface IRoomRepository : IGenericRepository<DoAn.HotelParking.Core.Domain.Entities.Hotel.Room>
{
	Task<DoAn.HotelParking.Core.Domain.Entities.Hotel.Room?> GetByIdWithHotelAsync(int id, CancellationToken cancellationToken = default);

	Task<IEnumerable<DoAn.HotelParking.Core.Domain.Entities.Hotel.Room>> GetByRoomTypeIdWithDetailsAsync(
		int roomTypeId,
		CancellationToken cancellationToken = default);
}
