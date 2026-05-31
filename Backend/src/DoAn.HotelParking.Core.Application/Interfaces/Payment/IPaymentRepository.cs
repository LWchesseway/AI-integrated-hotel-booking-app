using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Domain.Entities.Booking;

namespace DoAn.HotelParking.Core.Application.Interfaces.Payment;

public interface IPaymentRepository : IGenericRepository<DoAn.HotelParking.Core.Domain.Entities.Booking.Payment>
{
}
