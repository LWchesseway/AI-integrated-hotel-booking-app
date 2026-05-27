using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Location;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Location;
using DoAn.HotelParking.Core.Domain.Entities.Location;

namespace DoAn.HotelParking.Core.Application.Services.Location;

public class ProvinceService : IProvinceService
{
    private readonly IProvinceRepository _provinceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProvinceService(
        IProvinceRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _provinceRepository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProvinceDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _provinceRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<ProvinceDto>>(entities);
    }

    public async Task<(IEnumerable<ProvinceDto> Items, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _provinceRepository.GetPagedAsync(pageIndex, pageSize, null, cancellationToken);
        return (_mapper.Map<IEnumerable<ProvinceDto>>(items), totalCount);
    }

    public async Task<ProvinceDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _provinceRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? default : _mapper.Map<ProvinceDto>(entity);
    }

    public async Task<ProvinceDto> CreateAsync(CreateProvinceDto dto, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<Province>(dto);
        await _provinceRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ProvinceDto>(entity);
    }

    public async Task<ProvinceDto?> UpdateAsync(int id, UpdateProvinceDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _provinceRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return default;
        }

        _mapper.Map(dto, entity);
        _provinceRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ProvinceDto>(entity);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _provinceRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        _provinceRepository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
