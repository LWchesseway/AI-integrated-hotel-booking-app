using DoAn.HotelParking.Core.Application.DTOs.Parking;
using DoAn.HotelParking.Core.Application.Interfaces.Parking;
using DoAn.HotelParking.Presentation.Api.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.Parking;

[Route("api/license-plate-logs")]
[Authorize]
public class LicensePlateLogsController : CrudControllerBase<LicensePlateLogDto, CreateLicensePlateLogDto, UpdateLicensePlateLogDto>
{
    public LicensePlateLogsController(ILicensePlateLogService service) : base(service)
    {
    }
}