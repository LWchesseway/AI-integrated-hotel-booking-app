using DoAn.HotelParking.Core.Application.DTOs.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Review;

namespace DoAn.HotelParking.Core.Application.Services.Review;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReviewService(IReviewRepository reviewRepository, IUnitOfWork unitOfWork)
    {
        _reviewRepository = reviewRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponseDto<IEnumerable<ReviewDto>>> GetAllReviewsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var reviews = await _reviewRepository.GetAllAsync(cancellationToken);
            var dtos = reviews.Select(MapToDto).ToList();
            return new ApiResponseDto<IEnumerable<ReviewDto>> { Success = true, Data = dtos };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<IEnumerable<ReviewDto>> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiPagedResponseDto<ReviewDto>> GetReviewsPagedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            var (reviews, totalCount) = await _reviewRepository.GetPagedAsync(pageIndex, pageSize, cancellationToken: cancellationToken);
            var dtos = reviews.Select(MapToDto).ToList();
            return new ApiPagedResponseDto<ReviewDto>
            {
                Success = true,
                Data = dtos,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            return new ApiPagedResponseDto<ReviewDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<ReviewDetailDto>> GetReviewByIdAsync(int reviewId, CancellationToken cancellationToken = default)
    {
        try
        {
            var review = await _reviewRepository.GetReviewDetailAsync(reviewId, cancellationToken);
            if (review == null)
                return new ApiResponseDto<ReviewDetailDto> { Success = false, Message = "Đánh giá không tìm thấy" };

            var dto = MapToDetailDto(review);
            return new ApiResponseDto<ReviewDetailDto> { Success = true, Data = dto };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<ReviewDetailDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<IEnumerable<ReviewDto>>> GetReviewsByRoomAsync(int roomId, CancellationToken cancellationToken = default)
    {
        try
        {
            var reviews = await _reviewRepository.GetReviewsByRoomAsync(roomId, cancellationToken);
            var dtos = reviews.Select(MapToDto).ToList();
            return new ApiResponseDto<IEnumerable<ReviewDto>> { Success = true, Data = dtos };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<IEnumerable<ReviewDto>> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<IEnumerable<ReviewDto>>> GetMyReviewsAsync(int customerId, CancellationToken cancellationToken = default)
    {
        try
        {
            var reviews = await _reviewRepository.GetReviewsByCustomerAsync(customerId, cancellationToken);
            var dtos = reviews.Select(MapToDto).ToList();
            return new ApiResponseDto<IEnumerable<ReviewDto>> { Success = true, Data = dtos };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<IEnumerable<ReviewDto>> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<RoomReviewStatsDto>> GetRoomReviewStatsAsync(int roomId, CancellationToken cancellationToken = default)
    {
        try
        {
            var room = await _context.Rooms.FindAsync([roomId], cancellationToken: cancellationToken);
            if (room == null)
                return new ApiResponseDto<RoomReviewStatsDto> { Success = false, Message = "Phòng không tìm thấy" };

            var reviews = await _reviewRepository.GetReviewsByRoomAsync(roomId, cancellationToken);

            var stats = new RoomReviewStatsDto
            {
                RoomId = roomId,
                RoomNumber = room.RoomNumber,
                TotalReviews = reviews.Count(),
                AverageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0,
                Rating5Count = reviews.Count(r => r.Rating == 5),
                Rating4Count = reviews.Count(r => r.Rating == 4),
                Rating3Count = reviews.Count(r => r.Rating == 3),
                Rating2Count = reviews.Count(r => r.Rating == 2),
                Rating1Count = reviews.Count(r => r.Rating == 1)
            };

            return new ApiResponseDto<RoomReviewStatsDto> { Success = true, Data = stats };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<RoomReviewStatsDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<ReviewDto>> CreateReviewAsync(CreateReviewDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Kiểm tra phòng tồn tại không
            var room = await _context.Rooms.FindAsync([dto.RoomId], cancellationToken: cancellationToken);
            if (room == null)
                return new ApiResponseDto<ReviewDto> { Success = false, Message = "Phòng không tìm thấy" };

            // Kiểm tra khách hàng tồn tại không
            var customer = await _context.Users.FindAsync([dto.CustomerId], cancellationToken: cancellationToken);
            if (customer == null)
                return new ApiResponseDto<ReviewDto> { Success = false, Message = "Khách hàng không tìm thấy" };

            // Kiểm tra đặt phòng tồn tại không
            var booking = await _context.Bookings.FindAsync([dto.BookingId], cancellationToken: cancellationToken);
            if (booking == null)
                return new ApiResponseDto<ReviewDto> { Success = false, Message = "Đặt phòng không tìm thấy" };

            // Kiểm tra rating hợp lệ (1-5)
            if (dto.Rating < 1 || dto.Rating > 5)
                return new ApiResponseDto<ReviewDto> { Success = false, Message = "Rating phải từ 1 đến 5" };

            // Kiểm tra khách hàng đã đánh giá phòng này chưa
            var existingReview = await _reviewRepository.HasCustomerReviewedRoomAsync(dto.CustomerId, dto.RoomId, cancellationToken);
            if (existingReview)
                return new ApiResponseDto<ReviewDto> { Success = false, Message = "Bạn đã đánh giá phòng này rồi" };

            var review = new Domain.Entities.Review.Review
            {
                BookingId = dto.BookingId,
                RoomId = dto.RoomId,
                CustomerId = dto.CustomerId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _reviewRepository.AddAsync(review, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            review.Room = room;
            review.Customer = customer;
            review.Booking = booking;
            var responseDto = MapToDto(review);
            return new ApiResponseDto<ReviewDto> { Success = true, Data = responseDto, Message = "Tạo đánh giá thành công" };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<ReviewDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<ReviewDto>> UpdateReviewAsync(int reviewId, UpdateReviewDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId, cancellationToken);
            if (review == null)
                return new ApiResponseDto<ReviewDto> { Success = false, Message = "Đánh giá không tìm thấy" };

            if (dto.Rating.HasValue)
            {
                if (dto.Rating < 1 || dto.Rating > 5)
                    return new ApiResponseDto<ReviewDto> { Success = false, Message = "Rating phải từ 1 đến 5" };
                review.Rating = dto.Rating.Value;
            }

            review.Comment = dto.Comment ?? review.Comment;

            _reviewRepository.Update(review);
            await _context.SaveChangesAsync(cancellationToken);

            var updatedReview = await _reviewRepository.GetReviewDetailAsync(reviewId, cancellationToken);
            var responseDto = MapToDto(updatedReview!);
            return new ApiResponseDto<ReviewDto> { Success = true, Data = responseDto, Message = "Cập nhật đánh giá thành công" };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<ReviewDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<bool>> DeleteReviewAsync(int reviewId, CancellationToken cancellationToken = default)
    {
        try
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId, cancellationToken);
            if (review == null)
                return new ApiResponseDto<bool> { Success = false, Message = "Đánh giá không tìm thấy" };

            _reviewRepository.Remove(review);
            await _context.SaveChangesAsync(cancellationToken);

            return new ApiResponseDto<bool> { Success = true, Data = true, Message = "Xóa đánh giá thành công" };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<bool> { Success = false, Message = ex.Message };
        }
    }

    private ReviewDto MapToDto(Domain.Entities.Review.Review review)
    {
        return new ReviewDto
        {
            Id = review.Id,
            BookingId = review.BookingId,
            RoomId = review.RoomId,
            RoomNumber = review.Room?.RoomNumber,
            CustomerId = review.CustomerId,
            CustomerName = $"{review.Customer?.FirstName} {review.Customer?.LastName}".Trim(),
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        };
    }

    private ReviewDetailDto MapToDetailDto(Domain.Entities.Review.Review review)
    {
        return new ReviewDetailDto
        {
            Id = review.Id,
            BookingId = review.BookingId,
            RoomId = review.RoomId,
            RoomNumber = review.Room?.RoomNumber,
            HotelName = review.Room?.Hotel?.Name,
            CustomerId = review.CustomerId,
            CustomerName = $"{review.Customer?.FirstName} {review.Customer?.LastName}".Trim(),
            CustomerPhone = review.Customer?.Phone,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        };
    }
}
