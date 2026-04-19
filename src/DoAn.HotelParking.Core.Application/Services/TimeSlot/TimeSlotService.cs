using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.TimeSlot;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.TimeSlot;
using TimeSlotEntity = DoAn.HotelParking.Core.Domain.Entities.Hotel.TimeSlot;

namespace DoAn.HotelParking.Core.Application.Services.TimeSlot;

public class TimeSlotService : ITimeSlotService
{
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TimeSlotService(
        ITimeSlotRepository timeSlotRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _timeSlotRepository = timeSlotRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TimeSlotDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _timeSlotRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<TimeSlotDto>>(items);
    }

    public async Task<(IEnumerable<TimeSlotDto> Items, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _timeSlotRepository.GetPagedAsync(pageIndex, pageSize, null, cancellationToken);
        return (_mapper.Map<IEnumerable<TimeSlotDto>>(items), totalCount);
    }

    public async Task<TimeSlotDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await _timeSlotRepository.GetByIdAsync(id, cancellationToken);
        return item is null ? default : _mapper.Map<TimeSlotDto>(item);
    }

    public async Task<IEnumerable<TimeSlotDto>> GetByHotelIdAsync(int hotelId, CancellationToken cancellationToken = default)
    {
        var items = await _timeSlotRepository.GetByHotelIdAsync(hotelId, cancellationToken);
        return _mapper.Map<IEnumerable<TimeSlotDto>>(items);
    }

    public async Task<TimeSlotDto> CreateAsync(CreateTimeSlotDto dto, CancellationToken cancellationToken = default)
    {
        ValidatePolicy(dto.MinStayNights, dto.MaxStayNights);

        if (dto.IsDefault)
        {
            await _timeSlotRepository.ClearDefaultAsync(dto.HotelId, null, cancellationToken);
        }

        var entity = _mapper.Map<TimeSlotEntity>(dto);
        await _timeSlotRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TimeSlotDto>(entity);
    }

    public async Task<TimeSlotDto?> UpdateAsync(int id, UpdateTimeSlotDto dto, CancellationToken cancellationToken = default)
    {
        ValidatePolicy(dto.MinStayNights, dto.MaxStayNights);

        var entity = await _timeSlotRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return default;
        }

        if (dto.IsDefault)
        {
            await _timeSlotRepository.ClearDefaultAsync(dto.HotelId, id, cancellationToken);
        }

        _mapper.Map(dto, entity);
        _timeSlotRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TimeSlotDto>(entity);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _timeSlotRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        _timeSlotRepository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static void ValidatePolicy(int minStayNights, int? maxStayNights)
    {
        if (minStayNights < 1)
        {
            throw new InvalidOperationException("MinStayNights must be greater than or equal to 1.");
        }

        if (maxStayNights.HasValue && maxStayNights.Value < minStayNights)
        {
            throw new InvalidOperationException("MaxStayNights must be greater than or equal to MinStayNights.");
        }
    }
}
