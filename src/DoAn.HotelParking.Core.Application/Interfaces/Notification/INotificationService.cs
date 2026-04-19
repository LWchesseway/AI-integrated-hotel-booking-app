using DoAn.HotelParking.Core.Application.DTOs.Notification;

namespace DoAn.HotelParking.Core.Application.Interfaces.Notification;

public interface INotificationService
{
	Task<IEnumerable<NotificationDto>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<(IEnumerable<NotificationDto> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default);
	Task<NotificationDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
	Task<NotificationDto> CreateAsync(CreateNotificationDto dto, CancellationToken cancellationToken = default);
	Task<NotificationDto?> UpdateAsync(int id, UpdateNotificationDto dto, CancellationToken cancellationToken = default);
	Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}