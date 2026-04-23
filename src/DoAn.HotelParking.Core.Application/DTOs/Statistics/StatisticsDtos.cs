namespace DoAn.HotelParking.Core.Application.DTOs.Statistics;

public class OwnerDashboardStatsDto
{

    public int TotalBookings { get; set; }
    public int TodayBookings { get; set; }
    public int PendingBookings { get; set; }
    public int CompletedBookings { get; set; }
    public int CancelledBookings { get; set; }
    public int TotalHotels { get; set; }
    public int TotalRooms { get; set; }
    public int ActiveRooms { get; set; }
    public decimal OccupancyRate { get; set; }
    public decimal AvgBookingValue { get; set; }
}

public class RevenueChartDto
{
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
    public int BookingCount { get; set; }
}

public enum RevenuePeriodType
{
    Daily,
    Weekly,
    Monthly,
    Quarterly,
    Yearly
}

public class RevenueSummaryDto
{
    public string Period { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal ConfirmedRevenue { get; set; }
    public decimal CompletedRevenue { get; set; }
    public int TotalBookings { get; set; }
    public int CompletedBookings { get; set; }
    public int ConfirmedBookings { get; set; }
}

public class RevenueComparisonDto
{
    public string CurrentPeriod { get; set; } = string.Empty;
    public string PreviousPeriod { get; set; } = string.Empty;
    public decimal CurrentRevenue { get; set; }
    public decimal PreviousRevenue { get; set; }
    public decimal ChangeAmount { get; set; }
    public decimal ChangePercentage { get; set; }
    public int CurrentBookings { get; set; }
    public int PreviousBookings { get; set; }
}

public class TopRoomDto
{
    public int RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public int BookingCount { get; set; }
    public decimal Revenue { get; set; }
}

public class PeakHourDto
{
    public string Hour { get; set; } = string.Empty;
    public int BookingCount { get; set; }
}

public class UpcomingBookingDto
{
    public int Id { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public string HotelName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalAmount { get; set; }
}
