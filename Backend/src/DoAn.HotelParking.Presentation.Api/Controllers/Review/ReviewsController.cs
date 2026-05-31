using DoAn.HotelParking.Core.Application.DTOs.Base;
using DoAn.HotelParking.Core.Application.DTOs.Review;
using DoAn.HotelParking.Core.Application.Interfaces.Review;
using DoAn.HotelParking.Presentation.Api.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.Review;

[Route("api/reviews")]
[Authorize]
[ApiController]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService service)
    {
        _reviewService = service;
    }

    /// <summary>
    /// Chuc nang: Lay danh sach danh gia theo phan trang.
    /// </summary>
    /// <param name="pageIndex">Dau vao: Chi so trang (query).</param>
    /// <param name="pageSize">Dau vao: Kich thuoc trang (query).</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua danh sach danh gia.</returns>
    [HttpGet]
    [HasPermission("review.read")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _reviewService.GetPagedAsync(pageIndex, pageSize, cancellationToken);
        return Ok(ApiPagedResponse<ReviewDto>.Ok(items, pageIndex, pageSize, totalCount));
    }

    /// <summary>
    /// Chuc nang: Lay thong tin danh gia theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id danh gia.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua danh gia neu tim thay.</returns>
    [HttpGet("{id:int}")]
    [HasPermission("review.read")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var item = await _reviewService.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound(ApiResponse<ReviewDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<ReviewDto>.Ok(item));
    }

    /// <summary>
    /// Chuc nang: Tao moi danh gia.
    /// </summary>
    /// <param name="dto">Dau vao: Du lieu tao danh gia.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua danh gia vua tao.</returns>
    [HttpPost]
    [HasPermission("review.manage")]
    public async Task<IActionResult> Create([FromBody] CreateReviewDto dto, CancellationToken cancellationToken = default)
    {
        var created = await _reviewService.CreateAsync(dto, cancellationToken);
        return Ok(ApiResponse<ReviewDto>.Ok(created, "Created", 201));
    }

    /// <summary>
    /// Chuc nang: Cap nhat danh gia theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id danh gia.</param>
    /// <param name="dto">Dau vao: Du lieu cap nhat danh gia.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua danh gia sau cap nhat.</returns>
    [HttpPut("{id:int}")]
    [HasPermission("review.manage")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateReviewDto dto, CancellationToken cancellationToken = default)
    {
        var updated = await _reviewService.UpdateAsync(id, dto, cancellationToken);
        if (updated is null)
        {
            return NotFound(ApiResponse<ReviewDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<ReviewDto>.Ok(updated, "Updated"));
    }

    /// <summary>
    /// Chuc nang: Xoa danh gia theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id danh gia.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult thong bao ket qua xoa.</returns>
    [HttpDelete("{id:int}")]
    [HasPermission("review.manage")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _reviewService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<object>.Ok(null, "Deleted"));
    }
}