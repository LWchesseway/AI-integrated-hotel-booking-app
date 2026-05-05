using System.IdentityModel.Tokens.Jwt;
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

    /// <summary>
    /// Chuc nang: Lay thong ke dashboard cho owner dang dang nhap.
    /// </summary>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua thong ke dashboard.</returns>
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardStats(CancellationToken cancellationToken = default)
    {
        var ownerId = GetCurrentUserId();
        var stats = await _statisticsService.GetOwnerDashboardStatsAsync(ownerId, cancellationToken);
        return Ok(ApiResponse<OwnerDashboardStatsDto>.Ok(stats, "Dashboard statistics retrieved"));
    }

    /// <summary>
    /// Chuc nang: Lay du lieu bieu do doanh thu trong khoang thoi gian.
    /// </summary>
    /// <param name="startDate">Dau vao: Ngay bat dau (query).</param>
    /// <param name="endDate">Dau vao: Ngay ket thuc (query).</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua du lieu doanh thu.</returns>
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

    /// <summary>
    /// Chuc nang: Lay danh sach phong top theo doanh thu/dat phong.
    /// </summary>
    /// <param name="limit">Dau vao: So luong phong can lay (query).</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua danh sach phong top.</returns>
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

    /// <summary>
    /// Chuc nang: Lay thong ke khung gio cao diem.
    /// </summary>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua danh sach khung gio cao diem.</returns>
    [HttpGet("peak-hours")]
    public async Task<IActionResult> GetPeakHours(CancellationToken cancellationToken = default)
    {
        var ownerId = GetCurrentUserId();
        var data = await _statisticsService.GetPeakHoursAsync(ownerId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<PeakHourDto>>.Ok(data, "Peak hours retrieved"));
    }

    /// <summary>
    /// Chuc nang: Lay danh sach booking sap toi trong khoang gio.
    /// </summary>
    /// <param name="hoursAhead">Dau vao: So gio sap toi (query).</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua danh sach booking sap toi.</returns>
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

    /// <summary>
    /// Chuc nang: Tong hop doanh thu theo ky va khoang thoi gian.
    /// </summary>
    /// <param name="periodType">Dau vao: Loai ky thong ke (query).</param>
    /// <param name="startDate">Dau vao: Ngay bat dau (query).</param>
    /// <param name="endDate">Dau vao: Ngay ket thuc (query).</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua tong hop doanh thu.</returns>
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

    /// <summary>
    /// Chuc nang: So sanh doanh thu theo ky va khoang thoi gian.
    /// </summary>
    /// <param name="periodType">Dau vao: Loai ky thong ke (query).</param>
    /// <param name="startDate">Dau vao: Ngay bat dau (query).</param>
    /// <param name="endDate">Dau vao: Ngay ket thuc (query).</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua thong tin so sanh doanh thu.</returns>
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
        var rawUserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (!int.TryParse(rawUserId, out var userId))
        {
            throw new UnauthorizedAccessException("Unable to resolve current user from token.");
        }

        return userId;
    }
}
