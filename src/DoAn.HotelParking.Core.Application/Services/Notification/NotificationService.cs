using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Notification;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Notification;
using DoAn.HotelParking.Core.Application.Services.Base;

namespace DoAn.HotelParking.Core.Application.Services.Notification;

public class NotificationService : CrudService<DoAn.HotelParking.Core.Domain.Entities.Notification.Notification, NotificationDto, CreateNotificationDto, UpdateNotificationDto>, INotificationService
{
    public NotificationService(
        INotificationRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
        : base(repository, unitOfWork, mapper)
    {
    }
}