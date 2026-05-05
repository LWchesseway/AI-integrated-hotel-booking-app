using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DoAn.HotelParking.Core.Application.DTOs.Hotel;
using DoAn.HotelParking.Core.Application.DTOs.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Hotel;
using DoAn.HotelParking.Presentation.Api.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.Hotel;

[Route("api/hotels")]
[Authorize]
[ApiController]
public class HotelsController : ControllerBase
{
    private readonly IHotelService _hotelService;

    public HotelsController(IHotelService service)
    {
        _hotelService = service;
    }

    
    /// <summary>
    /// Chuc nang: Lay danh sach khach san theo phan trang.
    /// </summary>
    /// <param name="pageIndex">Dau vao: Chi so trang (query).</param>
    /// <param name="pageSize">Dau vao: Kich thuoc trang (query).</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua danh sach khach san.</returns>
    [HttpGet]
    [Authorize(Roles = "Admin,Owner")]
    [HasPermission("hotel.read")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _hotelService.GetPagedAsync(pageIndex, pageSize, cancellationToken);
        return Ok(ApiPagedResponse<HotelDetailDto>.Ok(items, pageIndex, pageSize, totalCount));
    }

    /// <summary>
    /// Chuc nang: Lay danh sach khach san kem thong tin tinh/thanh.
    /// </summary>
    /// <param name="pageIndex">Dau vao: Chi so trang (query).</param>
    /// <param name="pageSize">Dau vao: Kich thuoc trang (query).</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua danh sach khach san.</returns>
    [HttpGet("all-with-province")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllWithProvince(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
       var (items, totalCount) = await _hotelService.GetPagedWithProvinceAsync(pageIndex, pageSize, cancellationToken);
       
        return Ok(ApiPagedResponse<HotelDetailDto>.Ok(items, pageIndex, pageSize, totalCount));
    }

    /// <summary>
    /// Chuc nang: Lay thong tin khach san theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id khach san.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua khach san neu tim thay.</returns>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Owner")]
    [HasPermission("hotel.read")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var item = await _hotelService.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound(ApiResponse<HotelDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<HotelDto>.Ok(item));
    }

    /// <summary>
    /// Chuc nang: Lay khach san theo id kem thong tin dia diem.
    /// </summary>
    /// <param name="id">Dau vao: Id khach san.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua khach san kem dia diem.</returns>
    [HttpGet("{id:int}/with-location")]
    [AllowAnonymous]
    public async Task<IActionResult> GetHotelWithLocation(int id, CancellationToken cancellationToken = default)
    {
        var item = await _hotelService.GetByIdWithLocationAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound(ApiResponse<HotelDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<HotelDto>.Ok(item));
    }

    /// <summary>
    /// Chuc nang: Tim khach san theo ten.
    /// </summary>
    /// <param name="hotelName">Dau vao: Ten khach san (query).</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua danh sach khach san phu hop.</returns>
    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> SearchByName(
        [FromQuery] string hotelName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(hotelName))
        {
            return BadRequest(ApiResponse<IEnumerable<HotelDetailDto>>.Fail("hotelName is required.", 400));
        }

        var items = await _hotelService.SearchByNameAsync(hotelName, cancellationToken);
        return Ok(ApiResponse<IEnumerable<HotelDetailDto>>.Ok(items));
    }

    /// <summary>
    /// Chuc nang: Lay danh sach khach san theo tinh/thanh.
    /// </summary>
    /// <param name="province">Dau vao: Ten tinh/thanh (query).</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua danh sach khach san.</returns>
    [HttpGet("by-province")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByProvince(
        [FromQuery] string province,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(province))
        {
            return BadRequest(ApiResponse<IEnumerable<HotelDetailDto>>.Fail("province is required.", 400));
        }

        var items = await _hotelService.GetByProvinceAsync(province, cancellationToken);
        return Ok(ApiResponse<IEnumerable<HotelDetailDto>>.Ok(items));
    }

    /// <summary>
    /// Chuc nang: Tao moi khach san (admin).
    /// </summary>
    /// <param name="dto">Dau vao: Du lieu tao khach san.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua khach san vua tao.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [HasPermission("hotel.manage")]
    public async Task<IActionResult> Create([FromBody] CreateHotelDto dto, CancellationToken cancellationToken = default)
    {
        var created = await _hotelService.CreateAsync(dto, cancellationToken);
        return Ok(ApiResponse<HotelDto>.Ok(created, "Created", 201));
    }

    /// <summary>
    /// Chuc nang: Cap nhat khach san theo id (admin).
    /// </summary>
    /// <param name="id">Dau vao: Id khach san.</param>
    /// <param name="dto">Dau vao: Du lieu cap nhat khach san.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua khach san sau cap nhat.</returns>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [HasPermission("hotel.manage")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateHotelDto dto, CancellationToken cancellationToken = default)
    {
        var updated = await _hotelService.UpdateAsync(id, dto, cancellationToken);
        if (updated is null)
        {
            return NotFound(ApiResponse<HotelDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<HotelDto>.Ok(updated, "Updated"));
    }

    /// <summary>
    /// Chuc nang: Xoa khach san theo id (admin).
    /// </summary>
    /// <param name="id">Dau vao: Id khach san.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult thong bao ket qua xoa.</returns>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [HasPermission("hotel.manage")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _hotelService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<object>.Ok(null, "Deleted"));
    }

    /// <summary>
    /// Chuc nang: Tao moi khach san cho owner dang dang nhap.
    /// </summary>
    /// <param name="dto">Dau vao: Du lieu tao khach san.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua khach san vua tao.</returns>
    [HttpPost("owner")]
    [Authorize(Roles = "Owner")]
    [HasPermission("hotel.manage")]
    public async Task<IActionResult> CreateOwnedHotel([FromBody] CreateHotelDto dto, CancellationToken cancellationToken = default)
    {
        var ownerId = GetCurrentUserId();
        var hotel = await _hotelService.CreateOwnedHotelAsync(ownerId, dto, cancellationToken);
        return Ok(ApiResponse<HotelDto>.Ok(hotel, "Created", 201));
    }

    /// <summary>
    /// Chuc nang: Cap nhat khach san cua owner dang dang nhap.
    /// </summary>
    /// <param name="id">Dau vao: Id khach san.</param>
    /// <param name="dto">Dau vao: Du lieu cap nhat khach san.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua khach san sau cap nhat.</returns>
    [HttpPut("owner/{id:int}")]
    [Authorize(Roles = "Owner")]
    [HasPermission("hotel.manage")]
    public async Task<IActionResult> UpdateOwnedHotel(int id, [FromBody] UpdateHotelDto dto, CancellationToken cancellationToken = default)
    {
        var ownerId = GetCurrentUserId();
        var hotel = await _hotelService.UpdateOwnedHotelAsync(id, ownerId, dto, cancellationToken);
        if (hotel is null)
        {
            return NotFound(ApiResponse<HotelDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<HotelDto>.Ok(hotel, "Updated"));
    }

    private int GetCurrentUserId()
    {
        var rawUserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (!int.TryParse(rawUserId, out var userId))
        {
            throw new UnauthorizedAccessException("Unable to resolve current user from token.");
        }

        return userId;
    }
}