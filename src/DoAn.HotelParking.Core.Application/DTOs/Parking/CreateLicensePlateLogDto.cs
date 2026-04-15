using System.ComponentModel.DataAnnotations;

namespace DoAn.HotelParking.Core.Application.DTOs.Parking;

public class CreateLicensePlateLogDto
{
    [Required]
    public int ParkingSessionId { get; set; }

    public string? ImageUrl { get; set; }

    [MaxLength(20)]
    public string? DetectedPlate { get; set; }

    [Range(typeof(decimal), "0", "1")]
    public decimal Confidence { get; set; }
}