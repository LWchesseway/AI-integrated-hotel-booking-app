using System.ComponentModel.DataAnnotations;
using DoAn.HotelParking.Core.Application.DTOs.Base;
using DoAn.HotelParking.Core.Application.DTOs.Hotel;
using DoAn.HotelParking.Core.Application.Interfaces.Hotel;
using DoAn.HotelParking.Presentation.Api.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.Hotel;

[ApiController]
[Route("api/hotels/{hotelId:int}/images")]
[Authorize(Roles = "Admin,Owner")]
public class HotelImagesController : ControllerBase
{
    private readonly IHotelImageService _hotelImageService;

    public HotelImagesController(IHotelImageService hotelImageService)
    {
        _hotelImageService = hotelImageService;
    }

    /// <summary>
    /// Chuc nang: Lay danh sach anh khach san theo hotelId.
    /// </summary>
    /// <param name="hotelId">Dau vao: Id khach san (route).</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua danh sach anh khach san.</returns>
    [HttpGet]
    [HasPermission("hotelimage.read")]
    public async Task<IActionResult> GetByHotelId(int hotelId, CancellationToken cancellationToken = default)
    {
        var items = await _hotelImageService.GetByHotelIdAsync(hotelId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<HotelImageDto>>.Ok(items));
    }

    /// <summary>
    /// Chuc nang: Tao moi anh khach san.
    /// </summary>
    /// <param name="hotelId">Dau vao: Id khach san (route).</param>
    /// <param name="dto">Dau vao: Du lieu tao anh khach san.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua anh khach san vua tao.</returns>
    [HttpPost]
    [HasPermission("hotelimage.manage")]
    public async Task<IActionResult> Create(
        int hotelId,
        [FromBody] CreateHotelImageDto dto,
        CancellationToken cancellationToken = default)
    {
        dto.HotelId = hotelId;
        var created = await _hotelImageService.CreateAsync(dto, cancellationToken);
        return Ok(ApiResponse<HotelImageDto>.Ok(created, "Created", 201));
    }

    /// <summary>
    /// Chuc nang: Upload file anh khach san.
    /// </summary>
    /// <param name="hotelId">Dau vao: Id khach san (route).</param>
    /// <param name="request">Dau vao: File anh va thong tin kem theo.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua anh khach san vua upload.</returns>
    [HttpPost("upload")]
    [HasPermission("hotelimage.manage")]
    public async Task<IActionResult> Upload(
        int hotelId,
        [FromForm] UploadHotelImageRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.File is null || request.File.Length == 0)
        {
            throw new InvalidOperationException("File is required.");
        }

        await using var stream = request.File.OpenReadStream();
        var created = await _hotelImageService.UploadAsync(
            hotelId,
            stream,
            request.File.Length,
            request.File.FileName,
            request.File.ContentType,
            request.IsPrimary,
            request.SortOrder,
            cancellationToken);

        return Ok(ApiResponse<HotelImageDto>.Ok(created, "Uploaded", 201));
    }

    /// <summary>
    /// Chuc nang: Cap nhat anh khach san theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id anh khach san.</param>
    /// <param name="dto">Dau vao: Du lieu cap nhat anh khach san.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua anh khach san sau cap nhat.</returns>
    [HttpPut("{id:int}")]
    [HasPermission("hotelimage.manage")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateHotelImageDto dto,
        CancellationToken cancellationToken = default)
    {
        var updated = await _hotelImageService.UpdateAsync(id, dto, cancellationToken);
        if (updated is null)
        {
            return NotFound(ApiResponse<HotelImageDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<HotelImageDto>.Ok(updated, "Updated"));
    }

    /// <summary>
    /// Chuc nang: Xoa anh khach san theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id anh khach san.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult thong bao ket qua xoa.</returns>
    [HttpDelete("{id:int}")]
    [HasPermission("hotelimage.manage")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _hotelImageService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<object>.Ok(null, "Deleted"));
    }
}

public class UploadHotelImageRequest
{
    [Required]
    public IFormFile File { get; set; } = null!;

    public bool IsPrimary { get; set; }

    public int SortOrder { get; set; }
}
