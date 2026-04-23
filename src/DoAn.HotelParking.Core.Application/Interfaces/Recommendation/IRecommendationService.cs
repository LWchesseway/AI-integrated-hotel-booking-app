using DoAn.HotelParking.Core.Application.DTOs.Recommendation;

namespace DoAn.HotelParking.Core.Application.Interfaces.Recommendation;

public interface IRecommendationService
{
    Task<RecommendationResponse> GetSimilarHotelsAsync(int hotelId, int topK = 10, CancellationToken cancellationToken = default);
    Task<RecommendationResponse> GetRecommendationsForNewUserAsync(string? province, string? ward, int topK = 10, CancellationToken cancellationToken = default);
    Task<RecommendationResponse> GetPersonalizedRecommendationsAsync(int userId, int topK = 10, string? province = null, CancellationToken cancellationToken = default);
}
