using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DoAn.HotelParking.Core.Application.DTOs.Base;
using DoAn.HotelParking.Core.Application.DTOs.Recommendation;
using DoAn.HotelParking.Core.Application.Interfaces.Recommendation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.Recommendation;

[ApiController]
[Route("api/recommendations")]
public class RecommendationsController : ControllerBase
{
    private readonly IRecommendationService _recommendationService;

    public RecommendationsController(IRecommendationService recommendationService)
    {
        _recommendationService = recommendationService;
    }

    [HttpGet("similar/{hotelId:int}")]
    public async Task<IActionResult> GetSimilarHotels(int hotelId, [FromQuery] int topK = 10, CancellationToken cancellationToken = default)
    {
        var result = await _recommendationService.GetSimilarHotelsAsync(hotelId, topK, cancellationToken);
        return Ok(ApiResponse<RecommendationResponse>.Ok(result, "Recommendations retrieved"));
    }

    [HttpGet("new-user")]
    public async Task<IActionResult> GetRecommendationsForNewUser(
        [FromQuery] string? province,
        [FromQuery] string? ward,
        [FromQuery] int topK = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _recommendationService.GetRecommendationsForNewUserAsync(province, ward, topK, cancellationToken);
        return Ok(ApiResponse<RecommendationResponse>.Ok(result, "Recommendations retrieved"));
    }

    [HttpGet("personalized")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetPersonalizedRecommendations(
        [FromQuery] string? province,
        [FromQuery] int topK = 10,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var result = await _recommendationService.GetPersonalizedRecommendationsAsync(userId, topK, province, cancellationToken);
        return Ok(ApiResponse<RecommendationResponse>.Ok(result, "Recommendations retrieved"));
    }

    [HttpGet("smart")]
    public async Task<IActionResult> GetSmartRecommendations(
        [FromQuery] string? province,
        [FromQuery] string? ward,
        [FromQuery] int topK = 10,
        CancellationToken cancellationToken = default)
    {
        var rawUserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (int.TryParse(rawUserId, out var userId))
        {
            var personalized = await _recommendationService.GetPersonalizedRecommendationsAsync(userId, topK, province, cancellationToken);
            if (personalized.Hotels.Count > 0)
            {
                return Ok(ApiResponse<RecommendationResponse>.Ok(personalized, "Personalized recommendations"));
            }
        }

        var fallback = await _recommendationService.GetRecommendationsForNewUserAsync(province, ward, topK, cancellationToken);
        return Ok(ApiResponse<RecommendationResponse>.Ok(fallback, "Location-based recommendations"));
    }

    private int GetCurrentUserId()
    {
        var rawUserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (!int.TryParse(rawUserId, out var userId))
        {
            throw new UnauthorizedAccessException("Unable to resolve current user from token.");
        }

        return userId;
    }
}
