using DoAn.HotelParking.Core.Application.DTOs.RoomType;
using DoAn.HotelParking.Core.Application.Interfaces.RoomType;
using DoAn.HotelParking.Presentation.Api.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.RoomType;

[Route("api/room-types")]
[Authorize(Roles = "Admin,Owner")]
public class RoomTypesController : CrudControllerBase<RoomTypeDto, CreateRoomTypeDto, UpdateRoomTypeDto>
{
    public RoomTypesController(IRoomTypeService service) : base(service)
    {
    }
}