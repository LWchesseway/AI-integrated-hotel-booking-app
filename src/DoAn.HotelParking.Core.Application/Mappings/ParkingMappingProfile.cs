using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Parking;
using DoAn.HotelParking.Core.Domain.Entities.Parking;
using DoAn.HotelParking.Core.Domain.Enums;

namespace DoAn.HotelParking.Core.Application.Mappings;

public class ParkingMappingProfile : Profile
{
    public ParkingMappingProfile()
    {
        CreateMap<ParkingSession, ParkingSessionDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (byte)src.Status));

        CreateMap<CreateParkingSessionDto, ParkingSession>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (ParkingSessionStatus)src.Status));

        CreateMap<UpdateParkingSessionDto, ParkingSession>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (ParkingSessionStatus)src.Status));

        CreateMap<LicensePlateLog, LicensePlateLogDto>();
        CreateMap<CreateLicensePlateLogDto, LicensePlateLog>();
        CreateMap<UpdateLicensePlateLogDto, LicensePlateLog>();
    }
}