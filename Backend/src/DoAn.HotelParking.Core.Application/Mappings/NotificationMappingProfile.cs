using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Notification;
using DoAn.HotelParking.Core.Domain.Entities.Notification;
using DoAn.HotelParking.Core.Domain.Enums;

namespace DoAn.HotelParking.Core.Application.Mappings;

public class NotificationMappingProfile : Profile
{
    public NotificationMappingProfile()
    {
        CreateMap<Notification, NotificationDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (byte)src.Type));

        CreateMap<CreateNotificationDto, Notification>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (NotificationType)src.Type));

        CreateMap<UpdateNotificationDto, Notification>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (NotificationType)src.Type));
    }
}