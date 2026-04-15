using System.ComponentModel.DataAnnotations;

namespace DoAn.HotelParking.Core.Application.DTOs.Parking;

public class UpdateParkingSessionDto
{
    public DateTime? CheckOutTime { get; set; }

    [MaxLength(20)]
    public string? CheckOutPlate { get; set; }

    public byte Status { get; set; }

    public int? VerifiedBy { get; set; }
}