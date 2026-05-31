using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Review;

namespace DoAn.HotelParking.Core.Application.Mappings;

public class ReviewMappingProfile : Profile
{
    public ReviewMappingProfile()
    {
        CreateMap<Core.Domain.Entities.Review.Review, ReviewDto>();
        CreateMap<CreateReviewDto, Core.Domain.Entities.Review.Review>();
        CreateMap<UpdateReviewDto, Core.Domain.Entities.Review.Review>();
    }
}