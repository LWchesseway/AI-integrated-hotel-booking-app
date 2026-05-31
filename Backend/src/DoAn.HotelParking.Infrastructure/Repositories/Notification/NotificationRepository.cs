using DoAn.HotelParking.Core.Application.Interfaces.Notification;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;

namespace DoAn.HotelParking.Infrastructure.Repositories.Notification;

public class NotificationRepository : GenericRepository<DoAn.HotelParking.Core.Domain.Entities.Notification.Notification>, INotificationRepository
{
    public NotificationRepository(ApplicationDbContext context) : base(context)
    {
    }
}