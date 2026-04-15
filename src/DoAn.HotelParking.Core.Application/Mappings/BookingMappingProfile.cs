using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Booking;
using DoAn.HotelParking.Core.Domain.Entities.Booking;
using DoAn.HotelParking.Core.Domain.Enums;

namespace DoAn.HotelParking.Core.Application.Mappings;

public class BookingMappingProfile : Profile
{
    public BookingMappingProfile()
    {
        CreateMap<Booking, BookingDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (byte)src.Status));

        CreateMap<CreateBookingDto, Booking>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (BookingStatus)src.Status));

        CreateMap<UpdateBookingDto, Booking>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (BookingStatus)src.Status));
    }
}