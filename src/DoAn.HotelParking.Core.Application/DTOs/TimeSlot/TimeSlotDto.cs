namespace DoAn.HotelParking.Core.Application.DTOs.TimeSlot;

public class TimeSlotDto
{
    public int Id { get; set; }
    public int HotelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public TimeOnly CheckInFrom { get; set; }
    public TimeOnly CheckOutUntil { get; set; }
    public int CancellationHoursBeforeCheckIn { get; set; }
    public int MinStayNights { get; set; }
    public int? MaxStayNights { get; set; }
    public bool IsDefault { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
}
