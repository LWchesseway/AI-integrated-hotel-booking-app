namespace DoAn.HotelParking.Core.Application.DTOs.Recommendation;

public class RecommendationResponse
{
    public string RecommendationType { get; set; } = null!;
    public List<HotelRecommendationDto> Hotels { get; set; } = new();
    public string? Message { get; set; }
}
