using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Parking;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Parking;
using DoAn.HotelParking.Core.Application.Services.Base;

namespace DoAn.HotelParking.Core.Application.Services.Parking;

public class ParkingSessionService : CrudService<DoAn.HotelParking.Core.Domain.Entities.Parking.ParkingSession, ParkingSessionDto, CreateParkingSessionDto, UpdateParkingSessionDto>, IParkingSessionService
{
    public ParkingSessionService(
        IParkingSessionRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
        : base(repository, unitOfWork, mapper)
    {
    }
}