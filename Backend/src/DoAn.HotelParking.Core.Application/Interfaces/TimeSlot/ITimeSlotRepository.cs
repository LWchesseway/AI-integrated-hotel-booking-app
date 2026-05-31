using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Domain.Entities.Hotel;

namespace DoAn.HotelParking.Core.Application.Interfaces.TimeSlot;

public interface ITimeSlotRepository : IGenericRepository<DoAn.HotelParking.Core.Domain.Entities.Hotel.TimeSlot>
{
    Task<DoAn.HotelParking.Core.Domain.Entities.Hotel.TimeSlot?> GetActiveByRoomAndDateRangeAsync(
        int roomId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<DoAn.HotelParking.Core.Domain.Entities.Hotel.TimeSlot>> GetByRoomIdAsync(int roomId, CancellationToken cancellationToken = default);
    Task<IEnumerable<DoAn.HotelParking.Core.Domain.Entities.Hotel.TimeSlot>> GetByHotelIdAsync(int hotelId, CancellationToken cancellationToken = default);
}
