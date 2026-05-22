using DoAn.HotelParking.Core.Application.Interfaces.Payment;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;

namespace DoAn.HotelParking.Infrastructure.Repositories.Payment;

public class PaymentRepository : GenericRepository<DoAn.HotelParking.Core.Domain.Entities.Booking.Payment>, IPaymentRepository
{
    public PaymentRepository(ApplicationDbContext context) : base(context)
    {
    }
}