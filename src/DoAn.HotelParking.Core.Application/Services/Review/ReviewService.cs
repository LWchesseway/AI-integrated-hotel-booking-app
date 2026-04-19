using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Review;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Review;
using ReviewEntity = DoAn.HotelParking.Core.Domain.Entities.Review.Review;

namespace DoAn.HotelParking.Core.Application.Services.Review;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ReviewService(
        IReviewRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _reviewRepository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReviewDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _reviewRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<ReviewDto>>(entities);
    }

    public async Task<(IEnumerable<ReviewDto> Items, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _reviewRepository.GetPagedAsync(pageIndex, pageSize, null, cancellationToken);
        return (_mapper.Map<IEnumerable<ReviewDto>>(items), totalCount);
    }

    public async Task<ReviewDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _reviewRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? default : _mapper.Map<ReviewDto>(entity);
    }

    public async Task<ReviewDto> CreateAsync(CreateReviewDto dto, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<ReviewEntity>(dto);
        await _reviewRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ReviewDto>(entity);
    }

    public async Task<ReviewDto?> UpdateAsync(int id, UpdateReviewDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _reviewRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return default;
        }

        _mapper.Map(dto, entity);
        _reviewRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ReviewDto>(entity);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _reviewRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        _reviewRepository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}