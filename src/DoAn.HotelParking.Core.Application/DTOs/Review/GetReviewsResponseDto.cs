namespace DoAn.HotelParking.Core.Application.DTOs.Review;

public class GetReviewsResponseDto
{
    public List<ReviewDto> Reviews { get; set; } = new();
    public ReviewStatisticsDto Statistics { get; set; } = new();
}
