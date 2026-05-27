using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.TimeSlot;

namespace DoAn.HotelParking.Core.Application.Mappings;

public class TimeSlotMappingProfile : Profile
{
    public TimeSlotMappingProfile()
    {
        CreateMap<DoAn.HotelParking.Core.Domain.Entities.Hotel.TimeSlot, TimeSlotDto>();
        CreateMap<CreateTimeSlotDto, DoAn.HotelParking.Core.Domain.Entities.Hotel.TimeSlot>();
        CreateMap<UpdateTimeSlotDto, DoAn.HotelParking.Core.Domain.Entities.Hotel.TimeSlot>();
    }
}
