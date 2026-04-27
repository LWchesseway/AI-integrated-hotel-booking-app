using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Hotel;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Hotel;
using HotelEntity = DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel;

namespace DoAn.HotelParking.Core.Application.Services.Hotel;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public HotelService(
        IHotelRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _hotelRepository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<HotelDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _hotelRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<HotelDto>>(entities);
    }

    public async Task<(IEnumerable<HotelDto> Items, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _hotelRepository.GetPagedAsync(pageIndex, pageSize, null, cancellationToken);
        return (_mapper.Map<IEnumerable<HotelDto>>(items), totalCount);
    }

    public async Task<HotelDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _hotelRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? default : _mapper.Map<HotelDto>(entity);
    }

    public async Task<IEnumerable<HotelDetailDto>> SearchByNameAsync(string hotelName, CancellationToken cancellationToken = default)
    {
        var entities = await _hotelRepository.SearchByNameWithLocationAsync(hotelName, cancellationToken);
        return _mapper.Map<IEnumerable<HotelDetailDto>>(entities);
    }

    public async Task<IEnumerable<HotelDetailDto>> GetByProvinceAsync(string province, CancellationToken cancellationToken = default)
    {
        var entities = await _hotelRepository.GetByProvinceWithLocationAsync(province, cancellationToken);
        return _mapper.Map<IEnumerable<HotelDetailDto>>(entities);
    }

    public async Task<HotelDto> CreateAsync(CreateHotelDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.OwnerId is null || dto.OwnerId <= 0)
        {
            throw new InvalidOperationException("OwnerId is required when creating hotel from admin flow.");
        }

        var entity = _mapper.Map<HotelEntity>(dto);
        entity.OwnerId = dto.OwnerId.Value;

        await _hotelRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<HotelDto>(entity);
    }

    public async Task<HotelDto> CreateOwnedHotelAsync(int ownerId, CreateHotelDto dto, CancellationToken cancellationToken = default)
    {
        dto.OwnerId = ownerId;
        return await CreateAsync(dto, cancellationToken);
    }

    public async Task<HotelDto?> UpdateAsync(int id, UpdateHotelDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _hotelRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return default;
        }

        _mapper.Map(dto, entity);

        if (dto.OwnerId is > 0)
        {
            entity.OwnerId = dto.OwnerId.Value;
        }

        entity.UpdatedAt = DateTime.UtcNow;

        _hotelRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<HotelDto>(entity);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _hotelRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        _hotelRepository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<HotelDto?> UpdateOwnedHotelAsync(int hotelId, int ownerId, UpdateHotelDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _hotelRepository.GetByIdAsync(hotelId, cancellationToken);
        if (entity is null || entity.OwnerId != ownerId)
        {
            return default;
        }

        dto.OwnerId = ownerId;
        return await UpdateAsync(hotelId, dto, cancellationToken);
    }
}