using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Domain.Entities.Booking;

namespace DoAn.HotelParking.Core.Application.Interfaces.Booking;

public interface IBookingRepository : IGenericRepository<DoAn.HotelParking.Core.Domain.Entities.Booking.Booking>
{
}
