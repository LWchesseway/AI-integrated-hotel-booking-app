using DoAn.HotelParking.Core.Application.DTOs.Review;
using DoAn.HotelParking.Core.Application.Interfaces.Review;
using DoAn.HotelParking.Presentation.Api.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.Review;

[Route("api/reviews")]
[Authorize]
public class ReviewsController : CrudControllerBase<ReviewDto, CreateReviewDto, UpdateReviewDto>
{
    public ReviewsController(IReviewService service) : base(service)
    {
    }
}