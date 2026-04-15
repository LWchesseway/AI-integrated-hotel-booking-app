using DoAn.HotelParking.Core.Application.DTOs.Payment;
using DoAn.HotelParking.Core.Application.Interfaces.Payment;
using DoAn.HotelParking.Presentation.Api.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.Payment;

[Route("api/payments")]
[Authorize]
public class PaymentsController : CrudControllerBase<PaymentDto, CreatePaymentDto, UpdatePaymentDto>
{
    public PaymentsController(IPaymentService service) : base(service)
    {
    }
}