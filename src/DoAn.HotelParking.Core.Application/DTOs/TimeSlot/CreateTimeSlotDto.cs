using System.ComponentModel.DataAnnotations;

namespace DoAn.HotelParking.Core.Application.DTOs.TimeSlot;

public class CreateTimeSlotDto
{
    [Required]
    public int HotelId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public TimeOnly CheckInFrom { get; set; }

    [Required]
    public TimeOnly CheckOutUntil { get; set; }

    [Range(0, 720)]
    public int CancellationHoursBeforeCheckIn { get; set; } = 24;

    [Range(1, 365)]
    public int MinStayNights { get; set; } = 1;

    [Range(1, 365)]
    public int? MaxStayNights { get; set; }

    public bool IsDefault { get; set; }
}
