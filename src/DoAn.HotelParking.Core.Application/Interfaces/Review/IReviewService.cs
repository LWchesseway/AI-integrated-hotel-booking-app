using DoAn.HotelParking.Core.Application.DTOs.Review;

namespace DoAn.HotelParking.Core.Application.Interfaces.Review;

public interface IReviewService
{
	Task<IEnumerable<ReviewDto>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<(IEnumerable<ReviewDto> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default);
	Task<ReviewDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
	Task<ReviewDto> CreateAsync(CreateReviewDto dto, CancellationToken cancellationToken = default);
	Task<ReviewDto?> UpdateAsync(int id, UpdateReviewDto dto, CancellationToken cancellationToken = default);
	Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}