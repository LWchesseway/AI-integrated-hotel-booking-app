using DoAn.HotelParking.Core.Application.DTOs.Payment;
using DoAn.HotelParking.Core.Application.Interfaces.Base;

namespace DoAn.HotelParking.Core.Application.Interfaces.Payment;

public interface IPaymentService : ICrudService<PaymentDto, CreatePaymentDto, UpdatePaymentDto>
{
}