using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Domain.Entities.Parking;

namespace DoAn.HotelParking.Core.Application.Interfaces.Parking;

public interface IParkingSessionRepository : IGenericRepository<DoAn.HotelParking.Core.Domain.Entities.Parking.ParkingSession>
{
}
