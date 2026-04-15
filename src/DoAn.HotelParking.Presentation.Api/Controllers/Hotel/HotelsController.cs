using DoAn.HotelParking.Core.Application.DTOs.Hotel;
using DoAn.HotelParking.Core.Application.Interfaces.Hotel;
using DoAn.HotelParking.Presentation.Api.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.Hotel;

[Route("api/hotels")]
[Authorize(Roles = "Admin,Owner")]
public class HotelsController : CrudControllerBase<HotelDto, CreateHotelDto, UpdateHotelDto>
{
    public HotelsController(IHotelService service) : base(service)
    {
    }
}