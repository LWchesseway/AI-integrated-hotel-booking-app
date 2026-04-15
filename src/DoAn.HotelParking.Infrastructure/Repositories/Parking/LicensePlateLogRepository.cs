using DoAn.HotelParking.Core.Application.Interfaces.Parking;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;

namespace DoAn.HotelParking.Infrastructure.Repositories.Parking;

public class LicensePlateLogRepository : GenericRepository<DoAn.HotelParking.Core.Domain.Entities.Parking.LicensePlateLog>, ILicensePlateLogRepository
{
    public LicensePlateLogRepository(ApplicationDbContext context) : base(context)
    {
    }
}