using DoAn.HotelParking.Core.Application.DTOs.Base;
using DoAn.HotelParking.Core.Application.DTOs.Location;
using DoAn.HotelParking.Core.Application.Interfaces.Location;
using DoAn.HotelParking.Presentation.Api.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.Location;

[Route("api/provinces")]
[Authorize(Roles = "Admin")]
[ApiController]
public class ProvincesController : ControllerBase
{
    private readonly IProvinceService _provinceService;

    public ProvincesController(IProvinceService service)
    {
        _provinceService = service;
    }

    /// <summary>
    /// Chuc nang: Lay danh sach tinh/thanh theo phan trang.
    /// </summary>
    /// <param name="pageIndex">Dau vao: Chi so trang (query).</param>
    /// <param name="pageSize">Dau vao: Kich thuoc trang (query).</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua danh sach tinh/thanh.</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _provinceService.GetPagedAsync(pageIndex, pageSize, cancellationToken);
        return Ok(ApiPagedResponse<ProvinceDto>.Ok(items, pageIndex, pageSize, totalCount));
    }

    /// <summary>
    /// Chuc nang: Lay thong tin tinh/thanh theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id tinh/thanh.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua tinh/thanh neu tim thay.</returns>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var item = await _provinceService.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound(ApiResponse<ProvinceDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<ProvinceDto>.Ok(item));
    }

    /// <summary>
    /// Chuc nang: Tao moi tinh/thanh.
    /// </summary>
    /// <param name="dto">Dau vao: Du lieu tao tinh/thanh.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua tinh/thanh vua tao.</returns>
    [HttpPost]
    [HasPermission("location.manage")]
    public async Task<IActionResult> Create([FromBody] CreateProvinceDto dto, CancellationToken cancellationToken = default)
    {
        var created = await _provinceService.CreateAsync(dto, cancellationToken);
        return Ok(ApiResponse<ProvinceDto>.Ok(created, "Created", 201));
    }

    /// <summary>
    /// Chuc nang: Cap nhat tinh/thanh theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id tinh/thanh.</param>
    /// <param name="dto">Dau vao: Du lieu cap nhat tinh/thanh.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua tinh/thanh sau cap nhat.</returns>
    [HttpPut("{id:int}")]
    [HasPermission("location.manage")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProvinceDto dto, CancellationToken cancellationToken = default)
    {
        var updated = await _provinceService.UpdateAsync(id, dto, cancellationToken);
        if (updated is null)
        {
            return NotFound(ApiResponse<ProvinceDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<ProvinceDto>.Ok(updated, "Updated"));
    }

    /// <summary>
    /// Chuc nang: Xoa tinh/thanh theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id tinh/thanh.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult thong bao ket qua xoa.</returns>
    [HttpDelete("{id:int}")]
    [HasPermission("location.manage")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _provinceService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<object>.Ok(null, "Deleted"));
    }
}
