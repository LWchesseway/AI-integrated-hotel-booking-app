using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Payment;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Payment;
using DoAn.HotelParking.Core.Application.Services.Base;

namespace DoAn.HotelParking.Core.Application.Services.Payment;

public class PaymentService : CrudService<DoAn.HotelParking.Core.Domain.Entities.Booking.Payment, PaymentDto, CreatePaymentDto, UpdatePaymentDto>, IPaymentService
{
    public PaymentService(
        IPaymentRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
        : base(repository, unitOfWork, mapper)
    {
    }
}