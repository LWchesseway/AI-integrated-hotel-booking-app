using DoAn.HotelParking.Core.Application.DTOs.Review;
using DoAn.HotelParking.Core.Application.Interfaces.Base;

namespace DoAn.HotelParking.Core.Application.Interfaces.Review;

public interface IReviewService : ICrudService<ReviewDto, CreateReviewDto, UpdateReviewDto>
{
}