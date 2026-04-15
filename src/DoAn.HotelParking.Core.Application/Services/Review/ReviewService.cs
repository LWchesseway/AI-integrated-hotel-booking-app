using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Review;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Review;
using DoAn.HotelParking.Core.Application.Services.Base;

namespace DoAn.HotelParking.Core.Application.Services.Review;

public class ReviewService : CrudService<DoAn.HotelParking.Core.Domain.Entities.Review.Review, ReviewDto, CreateReviewDto, UpdateReviewDto>, IReviewService
{
    public ReviewService(
        IReviewRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
        : base(repository, unitOfWork, mapper)
    {
    }
}