using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Hotel;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Hotel;
using DoAn.HotelParking.Core.Application.Interfaces.Storage;
using DoAn.HotelParking.Core.Domain.Entities.Hotel;

namespace DoAn.HotelParking.Core.Application.Services.Hotel;

public class HotelImageService : IHotelImageService
{
    private readonly IHotelImageRepository _hotelImageRepository;
    private readonly IObjectStorageService _objectStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public HotelImageService(
        IHotelImageRepository hotelImageRepository,
        IObjectStorageService objectStorageService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _hotelImageRepository = hotelImageRepository;
        _objectStorageService = objectStorageService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<HotelImageDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _hotelImageRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<HotelImageDto>>(entities);
    }

    public async Task<(IEnumerable<HotelImageDto> Items, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _hotelImageRepository.GetPagedAsync(pageIndex, pageSize, null, cancellationToken);
        return (_mapper.Map<IEnumerable<HotelImageDto>>(items), totalCount);
    }

    public async Task<HotelImageDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _hotelImageRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? default : _mapper.Map<HotelImageDto>(entity);
    }

    public async Task<IEnumerable<HotelImageDto>> GetByHotelIdAsync(int hotelId, CancellationToken cancellationToken = default)
    {
        var entities = await _hotelImageRepository.GetByHotelIdAsync(hotelId, cancellationToken);
        return _mapper.Map<IEnumerable<HotelImageDto>>(entities);
    }

    public async Task<HotelImageDto> CreateAsync(CreateHotelImageDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.IsPrimary)
        {
            await _hotelImageRepository.ClearPrimaryAsync(dto.HotelId, null, cancellationToken);
        }

        var entity = _mapper.Map<HotelImage>(dto);
        await _hotelImageRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<HotelImageDto>(entity);
    }

    public async Task<HotelImageDto?> UpdateAsync(int id, UpdateHotelImageDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _hotelImageRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return default;
        }

        if (dto.IsPrimary)
        {
            await _hotelImageRepository.ClearPrimaryAsync(entity.HotelId, id, cancellationToken);
        }

        _mapper.Map(dto, entity);
        _hotelImageRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<HotelImageDto>(entity);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _hotelImageRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        _hotelImageRepository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<HotelImageDto> UploadAsync(
        int hotelId,
        Stream stream,
        long fileSize,
        string fileName,
        string contentType,
        bool isPrimary,
        int sortOrder,
        CancellationToken cancellationToken = default)
    {
        var objectKey = BuildObjectKey(hotelId, fileName);
        var imageUrl = await _objectStorageService.UploadAsync(stream, fileSize, objectKey, contentType, cancellationToken);

        var createDto = new CreateHotelImageDto
        {
            HotelId = hotelId,
            ImageUrl = imageUrl,
            ObjectKey = objectKey,
            IsPrimary = isPrimary,
            SortOrder = sortOrder
        };

        return await CreateAsync(createDto, cancellationToken);
    }

    private static string BuildObjectKey(int hotelId, string fileName)
    {
        var safeFileName = string.Concat(Path.GetFileName(fileName)
            .Select(ch => char.IsLetterOrDigit(ch) || ch is '.' or '-' or '_' ? ch : '-'));

        return $"hotels/{hotelId}/{Guid.NewGuid():N}-{safeFileName}";
    }
}
