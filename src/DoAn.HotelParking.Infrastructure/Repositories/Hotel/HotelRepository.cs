using DoAn.HotelParking.Core.Application.Interfaces.Hotel;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;

namespace DoAn.HotelParking.Infrastructure.Repositories.Hotel;

public class HotelRepository : GenericRepository<DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel>, IHotelRepository
{
    public HotelRepository(ApplicationDbContext context) : base(context)
    {
    }
}