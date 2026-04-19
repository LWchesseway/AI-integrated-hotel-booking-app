using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Domain.Entities.Hotel;

namespace DoAn.HotelParking.Core.Application.Interfaces.TimeSlot;

public interface ITimeSlotRepository : IGenericRepository<DoAn.HotelParking.Core.Domain.Entities.Hotel.TimeSlot>
{
    Task<DoAn.HotelParking.Core.Domain.Entities.Hotel.TimeSlot?> GetDefaultByHotelIdAsync(int hotelId, CancellationToken cancellationToken = default);
    Task<IEnumerable<DoAn.HotelParking.Core.Domain.Entities.Hotel.TimeSlot>> GetByHotelIdAsync(int hotelId, CancellationToken cancellationToken = default);
    Task ClearDefaultAsync(int hotelId, int? exceptTimeSlotId = null, CancellationToken cancellationToken = default);
}
