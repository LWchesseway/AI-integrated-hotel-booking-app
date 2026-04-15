using DoAn.HotelParking.Core.Application.Interfaces.Booking;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;

namespace DoAn.HotelParking.Infrastructure.Repositories.Booking;

public class BookingRepository : GenericRepository<DoAn.HotelParking.Core.Domain.Entities.Booking.Booking>, IBookingRepository
{
    public BookingRepository(ApplicationDbContext context) : base(context)
    {
    }
}