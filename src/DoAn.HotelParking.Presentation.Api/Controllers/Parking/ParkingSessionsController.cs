using DoAn.HotelParking.Core.Application.DTOs.Parking;
using DoAn.HotelParking.Core.Application.Interfaces.Parking;
using DoAn.HotelParking.Presentation.Api.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.Parking;

[Route("api/parking-sessions")]
[Authorize]
public class ParkingSessionsController : CrudControllerBase<ParkingSessionDto, CreateParkingSessionDto, UpdateParkingSessionDto>
{
    public ParkingSessionsController(IParkingSessionService service) : base(service)
    {
    }
}