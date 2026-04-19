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

    [HttpPost]
    [HasPermission("payment.manage")]
    public async Task<IActionResult> Create([FromBody] CreatePaymentDto dto, CancellationToken cancellationToken = default)
    {
        var created = await _paymentService.CreateAsync(dto, cancellationToken);
        return Ok(ApiResponse<PaymentDto>.Ok(created, "Created", 201));
    }

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