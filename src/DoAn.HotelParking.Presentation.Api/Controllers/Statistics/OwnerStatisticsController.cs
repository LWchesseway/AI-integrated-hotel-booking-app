using System.Security.Claims;
using DoAn.HotelParking.Core.Application.DTOs.Base;
using DoAn.HotelParking.Core.Application.DTOs.Statistics;
using DoAn.HotelParking.Core.Application.Interfaces.Statistics;
using DoAn.HotelParking.Presentation.Api.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.Statistics;

[ApiController]
[Route("api/statistics/owner")]
[Authorize(Roles = "Owner")]
[HasPermission("statistics.read")]
public class OwnerStatisticsController : ControllerBase
{
    private readonly IStatisticsService _statisticsService;

    public OwnerStatisticsController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardStats(CancellationToken cancellationToken = default)
    {
        var ownerId = GetCurrentUserId();
        var stats = await _statisticsService.GetOwnerDashboardStatsAsync(ownerId, cancellationToken);
        return Ok(ApiResponse<OwnerDashboardStatsDto>.Ok(stats, "Dashboard statistics retrieved"));
    }

    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenueChart(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var ownerId = GetCurrentUserId();
        var start = startDate ?? DateTime.UtcNow.Date.AddDays(-6);
        var end = endDate ?? DateTime.UtcNow.Date;

        if (end < start)
        {
            return BadRequest(ApiResponse<string>.Fail("endDate must be after startDate", 400));
        }

        if ((end - start).TotalDays > 365)
        {
            return BadRequest(ApiResponse<string>.Fail("Maximum range is 365 days", 400));
        }

        var data = await _statisticsService.GetOwnerRevenueChartAsync(ownerId, start, end, cancellationToken);
        return Ok(ApiResponse<IEnumerable<RevenueChartDto>>.Ok(data, "Revenue chart retrieved"));
    }

    [HttpGet("top-rooms")]
    public async Task<IActionResult> GetTopRooms([FromQuery] int limit = 5, CancellationToken cancellationToken = default)
    {
        var ownerId = GetCurrentUserId();

        if (limit < 1 || limit > 20)
        {
            return BadRequest(ApiResponse<string>.Fail("Limit must be between 1 and 20", 400));
        }

        var data = await _statisticsService.GetTopRoomsAsync(ownerId, limit, cancellationToken);
        return Ok(ApiResponse<IEnumerable<TopRoomDto>>.Ok(data, "Top rooms retrieved"));
    }

    [HttpGet("peak-hours")]
    public async Task<IActionResult> GetPeakHours(CancellationToken cancellationToken = default)
    {
        var ownerId = GetCurrentUserId();
        var data = await _statisticsService.GetPeakHoursAsync(ownerId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<PeakHourDto>>.Ok(data, "Peak hours retrieved"));
    }

    [HttpGet("upcoming")]
    public async Task<IActionResult> GetUpcomingBookings([FromQuery] int hoursAhead = 3, CancellationToken cancellationToken = default)
    {
        var ownerId = GetCurrentUserId();

        if (hoursAhead < 1 || hoursAhead > 72)
        {
            return BadRequest(ApiResponse<string>.Fail("hoursAhead must be between 1 and 72", 400));
        }

        var data = await _statisticsService.GetUpcomingBookingsAsync(ownerId, hoursAhead, cancellationToken);
        return Ok(ApiResponse<IEnumerable<UpcomingBookingDto>>.Ok(data, "Upcoming bookings retrieved"));
    }

    [HttpGet("revenue-summary")]
    public async Task<IActionResult> GetRevenueSummary(
        [FromQuery] RevenuePeriodType periodType = RevenuePeriodType.Daily,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var ownerId = GetCurrentUserId();
        var end = endDate ?? DateTime.UtcNow.Date;
        var start = startDate ?? periodType switch
        {
            RevenuePeriodType.Daily => end.AddDays(-30),
            RevenuePeriodType.Weekly => end.AddDays(-90),
            RevenuePeriodType.Monthly => end.AddMonths(-12),
            RevenuePeriodType.Quarterly => end.AddYears(-2),
            RevenuePeriodType.Yearly => end.AddYears(-5),
            _ => end.AddDays(-30)
        };

        var data = await _statisticsService.GetRevenueSummaryAsync(ownerId, periodType, start, end, cancellationToken);
        return Ok(ApiResponse<IEnumerable<RevenueSummaryDto>>.Ok(data, "Revenue summary retrieved"));
    }

    [HttpGet("revenue-comparison")]
    public async Task<IActionResult> GetRevenueComparison(
        [FromQuery] RevenuePeriodType periodType = RevenuePeriodType.Monthly,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var ownerId = GetCurrentUserId();
        var end = endDate ?? DateTime.UtcNow.Date;
        var start = startDate ?? periodType switch
        {
            RevenuePeriodType.Daily => end,
            RevenuePeriodType.Weekly => end.AddDays(-6),
            RevenuePeriodType.Monthly => new DateTime(end.Year, end.Month, 1),
            RevenuePeriodType.Quarterly => new DateTime(end.Year, ((end.Month - 1) / 3) * 3 + 1, 1),
            RevenuePeriodType.Yearly => new DateTime(end.Year, 1, 1),
            _ => end.AddDays(-30)
        };

        var comparison = await _statisticsService.GetRevenueComparisonAsync(ownerId, periodType, start, end, cancellationToken);
        return Ok(ApiResponse<RevenueComparisonDto>.Ok(comparison, "Revenue comparison retrieved"));
    }

    private int GetCurrentUserId()
    {
        var rawUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(rawUserId, out var userId))
        {
            throw new UnauthorizedAccessException("Unable to resolve current user from token.");
        }

        return userId;
    }
}
