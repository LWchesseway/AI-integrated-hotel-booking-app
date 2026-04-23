using DoAn.HotelParking.Core.Application.DTOs.TimeSlot;

namespace DoAn.HotelParking.Core.Application.Interfaces.TimeSlot;

public interface ITimeSlotService
{
    Task<IEnumerable<TimeSlotDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(IEnumerable<TimeSlotDto> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default);
    Task<TimeSlotDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TimeSlotDto> CreateAsync(CreateTimeSlotDto dto, CancellationToken cancellationToken = default);
    Task<TimeSlotDto?> UpdateAsync(int id, UpdateTimeSlotDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<IEnumerable<TimeSlotDto>> GetByRoomIdAsync(int roomId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TimeSlotDto>> GetByHotelIdAsync(int hotelId, CancellationToken cancellationToken = default);
}
