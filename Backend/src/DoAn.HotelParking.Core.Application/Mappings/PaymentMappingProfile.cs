using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Payment;
using DoAn.HotelParking.Core.Domain.Entities.Booking;
using DoAn.HotelParking.Core.Domain.Enums;

namespace DoAn.HotelParking.Core.Application.Mappings;

public class PaymentMappingProfile : Profile
{
    public PaymentMappingProfile()
    {
        CreateMap<Payment, PaymentDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (byte)src.Status));

        CreateMap<CreatePaymentDto, Payment>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (PaymentStatus)src.Status));

        CreateMap<UpdatePaymentDto, Payment>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (PaymentStatus)src.Status));
    }
}