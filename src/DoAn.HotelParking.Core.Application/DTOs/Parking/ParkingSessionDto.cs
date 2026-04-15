namespace DoAn.HotelParking.Core.Application.DTOs.Parking;

public class ParkingSessionDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int QrUserId { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string? CheckInPlate { get; set; }
    public string? CheckOutPlate { get; set; }
    public byte Status { get; set; }
    public int? VerifiedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}