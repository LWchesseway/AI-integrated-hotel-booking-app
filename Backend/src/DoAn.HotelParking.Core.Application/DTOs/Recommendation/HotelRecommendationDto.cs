namespace DoAn.HotelParking.Core.Application.DTOs.Recommendation;

public class HotelRecommendationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int OwnerId { get; set; }
    public string? Province { get; set; }
    public string? Ward { get; set; }
    public decimal? AvgRoomPrice { get; set; }
    public int RoomCount { get; set; }
    public double? AverageRating { get; set; }
    public int BookingCount { get; set; }
    public double SimilarityScore { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
}
