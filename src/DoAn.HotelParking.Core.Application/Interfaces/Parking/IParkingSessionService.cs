using DoAn.HotelParking.Core.Application.DTOs.Parking;
using DoAn.HotelParking.Core.Application.Interfaces.Base;

namespace DoAn.HotelParking.Core.Application.Interfaces.Parking;

public interface IParkingSessionService : ICrudService<ParkingSessionDto, CreateParkingSessionDto, UpdateParkingSessionDto>
{
}