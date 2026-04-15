using DoAn.HotelParking.Core.Application.Interfaces.Parking;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;

namespace DoAn.HotelParking.Infrastructure.Repositories.Parking;

public class ParkingSessionRepository : GenericRepository<DoAn.HotelParking.Core.Domain.Entities.Parking.ParkingSession>, IParkingSessionRepository
{
    public ParkingSessionRepository(ApplicationDbContext context) : base(context)
    {
    }
}