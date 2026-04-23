using DoAn.HotelParking.Core.Application.DTOs.Statistics;
using DoAn.HotelParking.Core.Application.Interfaces.Booking;
using DoAn.HotelParking.Core.Application.Interfaces.Statistics;
using DoAn.HotelParking.Core.Domain.Enums;
using BookingEntity = DoAn.HotelParking.Core.Domain.Entities.Booking.Booking;

namespace DoAn.HotelParking.Core.Application.Services.Statistics;

public class StatisticsService : IStatisticsService
{
    private readonly IBookingRepository _bookingRepository;

    public StatisticsService(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<OwnerDashboardStatsDto> GetOwnerDashboardStatsAsync(int ownerId, CancellationToken cancellationToken = default)
    {
        var allBookings = (await _bookingRepository.GetBookingsForOwnerAsync(ownerId, cancellationToken)).ToList();
        var today = DateTime.UtcNow.Date;

        var todayBookings = allBookings.Where(b => b.CheckInDate.Date == today).ToList();
        var pendingBookings = allBookings.Where(b => b.Status == BookingStatus.Pending).ToList();
        var completedBookings = allBookings.Where(b => b.Status == BookingStatus.Completed).ToList();
        var cancelledBookings = allBookings.Where(b => b.Status == BookingStatus.Cancelled).ToList();

        var totalRevenueBookings = allBookings
            .Where(b => b.Status is BookingStatus.Confirmed or BookingStatus.Completed)
            .ToList();

        var totalRevenue = totalRevenueBookings.Sum(GetRevenueFromBooking);
        var todayRevenue = todayBookings
            .Where(b => b.Status is BookingStatus.Confirmed or BookingStatus.Completed)
            .Sum(GetRevenueFromBooking);

        

        var rooms = allBookings.Select(b => b.Room).GroupBy(r => r.Id).Select(g => g.First()).ToList();
        var totalRooms = rooms.Count;
        var activeRooms = rooms.Count(r => r.Status == RoomStatus.Available && !r.IsDeleted);
        var totalHotels = rooms.Select(r => r.HotelId).Distinct().Count();

        var bookedRoomsToday = todayBookings
            .Where(b => b.Status != BookingStatus.Cancelled)
            .Select(b => b.RoomId)
            .Distinct()
            .Count();

        var occupancyRate = totalRooms > 0
            ? Math.Round((decimal)bookedRoomsToday / totalRooms * 100, 2)
            : 0;

        var avgBookingValue = totalRevenueBookings.Count > 0
            ? Math.Round(totalRevenue / totalRevenueBookings.Count, 0)
            : 0;

        return new OwnerDashboardStatsDto
        {
           
            
            TotalBookings = allBookings.Count,
            TodayBookings = todayBookings.Count,
            PendingBookings = pendingBookings.Count,
            CompletedBookings = completedBookings.Count,
            CancelledBookings = cancelledBookings.Count,
            TotalHotels = totalHotels,
            TotalRooms = totalRooms,
            ActiveRooms = activeRooms,
            OccupancyRate = occupancyRate,
            AvgBookingValue = avgBookingValue
        };
    }

    public async Task<IEnumerable<RevenueChartDto>> GetOwnerRevenueChartAsync(
        int ownerId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var bookings = await _bookingRepository.GetBookingsForOwnerAsync(ownerId, cancellationToken);

        var grouped = bookings
            .Where(b => b.Status is BookingStatus.Confirmed or BookingStatus.Completed)
            .Where(b => b.CheckInDate.Date >= startDate.Date && b.CheckInDate.Date <= endDate.Date)
            .GroupBy(b => b.CheckInDate.Date)
            .ToDictionary(
                g => g.Key,
                g => new RevenueChartDto
                {
                    Date = g.Key,
                    Revenue = g.Sum(GetRevenueFromBooking),
                    BookingCount = g.Count()
                });

        var chart = new List<RevenueChartDto>();
        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            chart.Add(grouped.TryGetValue(date, out var dayData)
                ? dayData
                : new RevenueChartDto { Date = date, Revenue = 0, BookingCount = 0 });
        }

        return chart;
    }

    public async Task<IEnumerable<TopRoomDto>> GetTopRoomsAsync(int ownerId, int limit = 5, CancellationToken cancellationToken = default)
    {
        var normalizedLimit = Math.Clamp(limit, 1, 20);
        var bookings = await _bookingRepository.GetBookingsForOwnerAsync(ownerId, cancellationToken);

        return bookings
            .Where(b => b.Status is BookingStatus.Confirmed or BookingStatus.Completed)
            .GroupBy(b => new { b.RoomId, b.Room.RoomNumber })
            .Select(g => new TopRoomDto
            {
                RoomId = g.Key.RoomId,
                RoomName = string.IsNullOrWhiteSpace(g.Key.RoomNumber) ? $"Room {g.Key.RoomId}" : g.Key.RoomNumber!,
                BookingCount = g.Count(),
                Revenue = g.Sum(GetRevenueFromBooking)
            })
            .OrderByDescending(x => x.BookingCount)
            .ThenByDescending(x => x.Revenue)
            .Take(normalizedLimit)
            .ToList();
    }

    public async Task<IEnumerable<PeakHourDto>> GetPeakHoursAsync(int ownerId, CancellationToken cancellationToken = default)
    {
        var bookings = await _bookingRepository.GetBookingsForOwnerAsync(ownerId, cancellationToken);

        return bookings
            .Where(b => b.Status is BookingStatus.Confirmed or BookingStatus.Completed)
            .GroupBy(b => b.CreatedAt.Hour)
            .Select(g => new PeakHourDto
            {
                Hour = $"{g.Key:00}:00",
                BookingCount = g.Count()
            })
            .OrderByDescending(x => x.BookingCount)
            .ToList();
    }

    public async Task<IEnumerable<UpcomingBookingDto>> GetUpcomingBookingsAsync(
        int ownerId,
        int hoursAhead = 3,
        CancellationToken cancellationToken = default)
    {
        var normalizedHoursAhead = Math.Clamp(hoursAhead, 1, 72);
        var now = DateTime.UtcNow;
        var future = now.AddHours(normalizedHoursAhead);

        var bookings = await _bookingRepository.GetBookingsForOwnerAsync(ownerId, cancellationToken);

        return bookings
            .Where(b => b.Status == BookingStatus.Confirmed)
            .Where(b => b.CheckInDate >= now && b.CheckInDate <= future)
            .OrderBy(b => b.CheckInDate)
            .Take(20)
            .Select(b => new UpcomingBookingDto
            {
                Id = b.Id,
                RoomName = string.IsNullOrWhiteSpace(b.Room.RoomNumber) ? $"Room {b.RoomId}" : b.Room.RoomNumber!,
                HotelName = b.Room.Hotel.Name ?? string.Empty,
                CustomerName = $"{b.Customer.LastName} {b.Customer.FirstName}".Trim(),
                CheckInDate = b.CheckInDate,
                CheckOutDate = b.CheckOutDate,
                TotalAmount = b.TotalAmount
            })
            .ToList();
    }

    public async Task<IEnumerable<RevenueSummaryDto>> GetRevenueSummaryAsync(
        int ownerId,
        RevenuePeriodType periodType,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var bookings = (await _bookingRepository.GetBookingsForOwnerAsync(ownerId, cancellationToken))
            .Where(b => b.Status is BookingStatus.Confirmed or BookingStatus.Completed)
            .Where(b => b.CheckInDate.Date >= startDate.Date && b.CheckInDate.Date <= endDate.Date)
            .ToList();

        return periodType switch
        {
            RevenuePeriodType.Daily => GroupByDaily(bookings, startDate, endDate),
            RevenuePeriodType.Weekly => GroupByWeekly(bookings, startDate, endDate),
            RevenuePeriodType.Monthly => GroupByMonthly(bookings, startDate, endDate),
            RevenuePeriodType.Quarterly => GroupByQuarterly(bookings, startDate, endDate),
            RevenuePeriodType.Yearly => GroupByYearly(bookings, startDate, endDate),
            _ => GroupByDaily(bookings, startDate, endDate)
        };
    }

    public async Task<RevenueComparisonDto> GetRevenueComparisonAsync(
        int ownerId,
        RevenuePeriodType periodType,
        DateTime currentStartDate,
        DateTime currentEndDate,
        CancellationToken cancellationToken = default)
    {
        var periodLength = (currentEndDate.Date - currentStartDate.Date).Days + 1;
        var previousEndDate = currentStartDate.Date.AddDays(-1);
        var previousStartDate = previousEndDate.AddDays(-periodLength + 1);

        var currentData = await GetRevenueSummaryAsync(ownerId, periodType, currentStartDate, currentEndDate, cancellationToken);
        var previousData = await GetRevenueSummaryAsync(ownerId, periodType, previousStartDate, previousEndDate, cancellationToken);

        var currentRevenue = currentData.Sum(x => x.TotalRevenue);
        var previousRevenue = previousData.Sum(x => x.TotalRevenue);

        var currentBookings = currentData.Sum(x => x.TotalBookings);
        var previousBookings = previousData.Sum(x => x.TotalBookings);

        var changeAmount = currentRevenue - previousRevenue;
        var changePercentage = previousRevenue > 0
            ? Math.Round((changeAmount / previousRevenue) * 100, 2)
            : 0;

        return new RevenueComparisonDto
        {
            CurrentPeriod = $"{currentStartDate:yyyy-MM-dd} - {currentEndDate:yyyy-MM-dd}",
            PreviousPeriod = $"{previousStartDate:yyyy-MM-dd} - {previousEndDate:yyyy-MM-dd}",
            CurrentRevenue = currentRevenue,
            PreviousRevenue = previousRevenue,
            ChangeAmount = changeAmount,
            ChangePercentage = changePercentage,
            CurrentBookings = currentBookings,
            PreviousBookings = previousBookings
        };
    }

    private static decimal GetRevenueFromBooking(BookingEntity booking)
    {
        return booking.Status switch
        {
            BookingStatus.Completed => booking.TotalAmount,
            
        };
    }

    private static List<RevenueSummaryDto> GroupByDaily(List<BookingEntity> bookings, DateTime startDate, DateTime endDate)
    {
        var grouped = bookings.GroupBy(b => b.CheckInDate.Date).ToDictionary(g => g.Key, g => g.ToList());
        var result = new List<RevenueSummaryDto>();

        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            var dayBookings = grouped.TryGetValue(date, out var values) ? values : new List<BookingEntity>();
            result.Add(ToSummary(dayBookings, date.ToString("yyyy-MM-dd"), date, date));
        }

        return result;
    }

    private static List<RevenueSummaryDto> GroupByWeekly(List<BookingEntity> bookings, DateTime startDate, DateTime endDate)
    {
        var result = new List<RevenueSummaryDto>();
        var weekStart = startDate.Date;

        while (weekStart <= endDate.Date)
        {
            var weekEnd = weekStart.AddDays(6);
            if (weekEnd > endDate.Date)
            {
                weekEnd = endDate.Date;
            }

            var weekBookings = bookings.Where(b => b.CheckInDate.Date >= weekStart && b.CheckInDate.Date <= weekEnd).ToList();
            var year = System.Globalization.ISOWeek.GetYear(weekStart);
            var week = System.Globalization.ISOWeek.GetWeekOfYear(weekStart);

            result.Add(ToSummary(weekBookings, $"{year}-W{week:00}", weekStart, weekEnd));
            weekStart = weekStart.AddDays(7);
        }

        return result;
    }

    private static List<RevenueSummaryDto> GroupByMonthly(List<BookingEntity> bookings, DateTime startDate, DateTime endDate)
    {
        var result = new List<RevenueSummaryDto>();
        var monthCursor = new DateTime(startDate.Year, startDate.Month, 1);
        var lastMonth = new DateTime(endDate.Year, endDate.Month, 1);

        while (monthCursor <= lastMonth)
        {
            var monthEnd = monthCursor.AddMonths(1).AddDays(-1);
            if (monthEnd > endDate.Date)
            {
                monthEnd = endDate.Date;
            }

            var monthBookings = bookings.Where(b => b.CheckInDate.Date >= monthCursor && b.CheckInDate.Date <= monthEnd).ToList();
            result.Add(ToSummary(monthBookings, monthCursor.ToString("yyyy-MM"), monthCursor, monthEnd));

            monthCursor = monthCursor.AddMonths(1);
        }

        return result;
    }

    private static List<RevenueSummaryDto> GroupByQuarterly(List<BookingEntity> bookings, DateTime startDate, DateTime endDate)
    {
        var result = new List<RevenueSummaryDto>();
        var quarterMonth = ((startDate.Month - 1) / 3) * 3 + 1;
        var quarterCursor = new DateTime(startDate.Year, quarterMonth, 1);

        while (quarterCursor <= endDate.Date)
        {
            var quarterEnd = quarterCursor.AddMonths(3).AddDays(-1);
            if (quarterEnd > endDate.Date)
            {
                quarterEnd = endDate.Date;
            }

            var quarterBookings = bookings.Where(b => b.CheckInDate.Date >= quarterCursor && b.CheckInDate.Date <= quarterEnd).ToList();
            var quarter = ((quarterCursor.Month - 1) / 3) + 1;
            result.Add(ToSummary(quarterBookings, $"{quarterCursor.Year}-Q{quarter}", quarterCursor, quarterEnd));

            quarterCursor = quarterCursor.AddMonths(3);
        }

        return result;
    }

    private static List<RevenueSummaryDto> GroupByYearly(List<BookingEntity> bookings, DateTime startDate, DateTime endDate)
    {
        var result = new List<RevenueSummaryDto>();
        var yearCursor = new DateTime(startDate.Year, 1, 1);

        while (yearCursor <= endDate.Date)
        {
            var yearEnd = new DateTime(yearCursor.Year, 12, 31);
            if (yearEnd > endDate.Date)
            {
                yearEnd = endDate.Date;
            }

            var yearBookings = bookings.Where(b => b.CheckInDate.Date >= yearCursor && b.CheckInDate.Date <= yearEnd).ToList();
            result.Add(ToSummary(yearBookings, yearCursor.Year.ToString(), yearCursor, yearEnd));

            yearCursor = yearCursor.AddYears(1);
        }

        return result;
    }

    private static RevenueSummaryDto ToSummary(List<BookingEntity> bookings, string period, DateTime startDate, DateTime endDate)
    {
        var confirmed = bookings.Where(b => b.Status == BookingStatus.Confirmed).ToList();
        var completed = bookings.Where(b => b.Status == BookingStatus.Completed).ToList();

        return new RevenueSummaryDto
        {
            Period = period,
            StartDate = startDate,
            EndDate = endDate,
            TotalRevenue = bookings.Sum(GetRevenueFromBooking),
            ConfirmedRevenue = confirmed.Sum(GetRevenueFromBooking),
            CompletedRevenue = completed.Sum(GetRevenueFromBooking),
            TotalBookings = bookings.Count,
            ConfirmedBookings = confirmed.Count,
            CompletedBookings = completed.Count
        };
    }
}
