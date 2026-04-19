using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.RoomType;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.RoomType;
using RoomTypeEntity = DoAn.HotelParking.Core.Domain.Entities.Hotel.RoomType;

namespace DoAn.HotelParking.Core.Application.Services.RoomType;

public class RoomTypeService : IRoomTypeService
{
    private readonly IRoomTypeRepository _roomTypeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RoomTypeService(
        IRoomTypeRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _roomTypeRepository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RoomTypeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _roomTypeRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<RoomTypeDto>>(entities);
    }

    public async Task<(IEnumerable<RoomTypeDto> Items, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _roomTypeRepository.GetPagedAsync(pageIndex, pageSize, null, cancellationToken);
        return (_mapper.Map<IEnumerable<RoomTypeDto>>(items), totalCount);
    }

    public async Task<RoomTypeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _roomTypeRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? default : _mapper.Map<RoomTypeDto>(entity);
    }

    public async Task<RoomTypeDto> CreateAsync(CreateRoomTypeDto dto, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<RoomTypeEntity>(dto);
        await _roomTypeRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<RoomTypeDto>(entity);
    }

    public async Task<RoomTypeDto?> UpdateAsync(int id, UpdateRoomTypeDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _roomTypeRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return default;
        }

        _mapper.Map(dto, entity);
        _roomTypeRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<RoomTypeDto>(entity);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _roomTypeRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        _roomTypeRepository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}