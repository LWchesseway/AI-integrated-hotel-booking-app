using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Notification;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Notification;
using NotificationEntity = DoAn.HotelParking.Core.Domain.Entities.Notification.Notification;

namespace DoAn.HotelParking.Core.Application.Services.Notification;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public NotificationService(
        INotificationRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _notificationRepository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NotificationDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _notificationRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<NotificationDto>>(entities);
    }

    public async Task<(IEnumerable<NotificationDto> Items, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _notificationRepository.GetPagedAsync(pageIndex, pageSize, null, cancellationToken);
        return (_mapper.Map<IEnumerable<NotificationDto>>(items), totalCount);
    }

    public async Task<NotificationDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _notificationRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? default : _mapper.Map<NotificationDto>(entity);
    }

    public async Task<NotificationDto> CreateAsync(CreateNotificationDto dto, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<NotificationEntity>(dto);
        await _notificationRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<NotificationDto>(entity);
    }

    public async Task<NotificationDto?> UpdateAsync(int id, UpdateNotificationDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _notificationRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return default;
        }

        _mapper.Map(dto, entity);
        _notificationRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<NotificationDto>(entity);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _notificationRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        _notificationRepository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}