using DoAn.HotelParking.Core.Application.Interfaces.Location;
using DoAn.HotelParking.Core.Domain.Entities.Location;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;

namespace DoAn.HotelParking.Infrastructure.Repositories.Location;

public class WardRepository : GenericRepository<Ward>, IWardRepository
{
    public WardRepository(ApplicationDbContext context) : base(context)
    {
    }
}
