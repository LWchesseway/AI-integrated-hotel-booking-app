using System.ComponentModel.DataAnnotations;

namespace DoAn.HotelParking.Core.Application.DTOs.Parking;

public class CreateParkingSessionDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int QrUserId { get; set; }

    public DateTime CheckInTime { get; set; } = DateTime.UtcNow;

    [MaxLength(20)]
    public string? CheckInPlate { get; set; }

    public byte Status { get; set; } = 0;

    public int? VerifiedBy { get; set; }
}