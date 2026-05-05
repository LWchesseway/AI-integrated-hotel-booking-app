using DoAn.HotelParking.Core.Application.DTOs.Base;
using DoAn.HotelParking.Core.Application.DTOs.Payment;
using DoAn.HotelParking.Core.Application.Interfaces.Payment;
using DoAn.HotelParking.Presentation.Api.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.Payment;

[Route("api/payments")]
[Authorize]
[ApiController]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService service)
    {
        _paymentService = service;
    }

    /// <summary>
    /// Chuc nang: Lay danh sach thanh toan theo phan trang.
    /// </summary>
    /// <param name="pageIndex">Dau vao: Chi so trang (query).</param>
    /// <param name="pageSize">Dau vao: Kich thuoc trang (query).</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua danh sach thanh toan.</returns>
    [HttpGet]
    [HasPermission("payment.read")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _paymentService.GetPagedAsync(pageIndex, pageSize, cancellationToken);
        return Ok(ApiPagedResponse<PaymentDto>.Ok(items, pageIndex, pageSize, totalCount));
    }

    /// <summary>
    /// Chuc nang: Lay thong tin thanh toan theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id thanh toan.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua thanh toan neu tim thay.</returns>
    [HttpGet("{id:int}")]
    [HasPermission("payment.read")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var item = await _paymentService.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound(ApiResponse<PaymentDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<PaymentDto>.Ok(item));
    }

    /// <summary>
    /// Chuc nang: Tao moi thanh toan.
    /// </summary>
    /// <param name="dto">Dau vao: Du lieu tao thanh toan.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua thanh toan vua tao.</returns>
    [HttpPost]
    [HasPermission("payment.manage")]
    public async Task<IActionResult> Create([FromBody] CreatePaymentDto dto, CancellationToken cancellationToken = default)
    {
        var created = await _paymentService.CreateAsync(dto, cancellationToken);
        return Ok(ApiResponse<PaymentDto>.Ok(created, "Created", 201));
    }

    /// <summary>
    /// Chuc nang: Cap nhat thanh toan theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id thanh toan.</param>
    /// <param name="dto">Dau vao: Du lieu cap nhat thanh toan.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua thanh toan sau cap nhat.</returns>
    [HttpPut("{id:int}")]
    [HasPermission("payment.manage")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePaymentDto dto, CancellationToken cancellationToken = default)
    {
        var updated = await _paymentService.UpdateAsync(id, dto, cancellationToken);
        if (updated is null)
        {
            return NotFound(ApiResponse<PaymentDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<PaymentDto>.Ok(updated, "Updated"));
    }

    /// <summary>
    /// Chuc nang: Xoa thanh toan theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id thanh toan.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult thong bao ket qua xoa.</returns>
    [HttpDelete("{id:int}")]
    [HasPermission("payment.manage")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _paymentService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<object>.Ok(null, "Deleted"));
    }
}