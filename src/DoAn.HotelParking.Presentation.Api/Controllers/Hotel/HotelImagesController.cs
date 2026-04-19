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

    [HttpGet]
    [HasPermission("hotelimage.read")]
    public async Task<IActionResult> GetByHotelId(int hotelId, CancellationToken cancellationToken = default)
    {
        var items = await _hotelImageService.GetByHotelIdAsync(hotelId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<HotelImageDto>>.Ok(items));
    }

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
