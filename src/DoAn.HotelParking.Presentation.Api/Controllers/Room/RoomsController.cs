using DoAn.HotelParking.Core.Application.DTOs.Room;
using DoAn.HotelParking.Core.Application.Interfaces.Room;
using DoAn.HotelParking.Presentation.Api.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.Room;

[Route("api/rooms")]
[Authorize(Roles = "Admin,Owner")]
public class RoomsController : CrudControllerBase<RoomDto, CreateRoomDto, UpdateRoomDto>
{
    public RoomsController(IRoomService service) : base(service)
    {
    }
}