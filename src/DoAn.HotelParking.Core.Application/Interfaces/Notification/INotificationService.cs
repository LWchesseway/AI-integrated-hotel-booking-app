using DoAn.HotelParking.Core.Application.DTOs.Notification;
using DoAn.HotelParking.Core.Application.Interfaces.Base;

namespace DoAn.HotelParking.Core.Application.Interfaces.Notification;

public interface INotificationService : ICrudService<NotificationDto, CreateNotificationDto, UpdateNotificationDto>
{
}