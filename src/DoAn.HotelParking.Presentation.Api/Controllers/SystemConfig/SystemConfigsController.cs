using DoAn.HotelParking.Core.Application.DTOs.Base;
using DoAn.HotelParking.Core.Application.DTOs.SystemConfig;
using DoAn.HotelParking.Core.Application.Interfaces.SystemConfig;
using DoAn.HotelParking.Presentation.Api.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.SystemConfig;

[ApiController]
[Route("api/admin/system-configs")]
[Authorize(Roles = "Admin")]
[HasPermission("system.manage")]
public class SystemConfigsController : ControllerBase
{
    private readonly ISystemConfigService _systemConfigService;

    public SystemConfigsController(ISystemConfigService systemConfigService)
    {
        _systemConfigService = systemConfigService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var configs = await _systemConfigService.GetAllConfigsAsync(cancellationToken);
        return Ok(ApiResponse<IEnumerable<SystemConfigDto>>.Ok(configs, "System configs retrieved"));
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> GetByKey(string key, CancellationToken cancellationToken = default)
    {
        var config = await _systemConfigService.GetConfigByKeyAsync(key, cancellationToken);
        if (config is null)
        {
            return NotFound(ApiResponse<SystemConfigDto>.Fail("Config not found", 404));
        }

        return Ok(ApiResponse<SystemConfigDto>.Ok(config, "System config retrieved"));
    }

    [HttpPut("{key}")]
    public async Task<IActionResult> Update(string key, [FromBody] UpdateSystemConfigDto dto, CancellationToken cancellationToken = default)
    {
        await _systemConfigService.UpdateConfigAsync(key, dto, cancellationToken);
        return Ok(ApiResponse<object>.Ok(null, "System config updated"));
    }
}
