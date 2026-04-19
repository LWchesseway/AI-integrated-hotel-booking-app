using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Location;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Location;
using DoAn.HotelParking.Core.Domain.Entities.Location;

namespace DoAn.HotelParking.Core.Application.Services.Location;

public class WardService : IWardService
{
    private readonly IWardRepository _wardRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public WardService(
        IWardRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _wardRepository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<WardDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _wardRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<WardDto>>(entities);
    }

    public async Task<(IEnumerable<WardDto> Items, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _wardRepository.GetPagedAsync(pageIndex, pageSize, null, cancellationToken);
        return (_mapper.Map<IEnumerable<WardDto>>(items), totalCount);
    }

    public async Task<WardDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _wardRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? default : _mapper.Map<WardDto>(entity);
    }

    public async Task<WardDto> CreateAsync(CreateWardDto dto, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<Ward>(dto);
        await _wardRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<WardDto>(entity);
    }

    public async Task<WardDto?> UpdateAsync(int id, UpdateWardDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _wardRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return default;
        }

        _mapper.Map(dto, entity);
        _wardRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<WardDto>(entity);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _wardRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        _wardRepository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
