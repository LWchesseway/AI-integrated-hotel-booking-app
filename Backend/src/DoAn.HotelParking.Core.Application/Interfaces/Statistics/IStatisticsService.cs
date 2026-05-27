using DoAn.HotelParking.Core.Application.DTOs.Statistics;

namespace DoAn.HotelParking.Core.Application.Interfaces.Statistics;

public interface IStatisticsService
{
    Task<OwnerDashboardStatsDto> GetOwnerDashboardStatsAsync(int ownerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RevenueChartDto>> GetOwnerRevenueChartAsync(int ownerId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<TopRoomDto>> GetTopRoomsAsync(int ownerId, int limit = 5, CancellationToken cancellationToken = default);
    Task<IEnumerable<PeakHourDto>> GetPeakHoursAsync(int ownerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UpcomingBookingDto>> GetUpcomingBookingsAsync(int ownerId, int hoursAhead = 3, CancellationToken cancellationToken = default);
    Task<IEnumerable<RevenueSummaryDto>> GetRevenueSummaryAsync(int ownerId, RevenuePeriodType periodType, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<RevenueComparisonDto> GetRevenueComparisonAsync(int ownerId, RevenuePeriodType periodType, DateTime currentStartDate, DateTime currentEndDate, CancellationToken cancellationToken = default);
}
