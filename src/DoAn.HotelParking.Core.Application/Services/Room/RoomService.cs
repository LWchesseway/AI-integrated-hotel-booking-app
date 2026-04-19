using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Room;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Room;
using RoomEntity = DoAn.HotelParking.Core.Domain.Entities.Hotel.Room;

namespace DoAn.HotelParking.Core.Application.Services.Room;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RoomService(
        IRoomRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _roomRepository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RoomDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _roomRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<RoomDto>>(entities);
    }

    public async Task<(IEnumerable<RoomDto> Items, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _roomRepository.GetPagedAsync(pageIndex, pageSize, null, cancellationToken);
        return (_mapper.Map<IEnumerable<RoomDto>>(items), totalCount);
    }

    public async Task<RoomDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _roomRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? default : _mapper.Map<RoomDto>(entity);
    }

    public async Task<RoomDto> CreateAsync(CreateRoomDto dto, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<RoomEntity>(dto);
        await _roomRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<RoomDto>(entity);
    }

    public async Task<RoomDto?> UpdateAsync(int id, UpdateRoomDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _roomRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return default;
        }

        _mapper.Map(dto, entity);
        _roomRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<RoomDto>(entity);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _roomRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        _roomRepository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}