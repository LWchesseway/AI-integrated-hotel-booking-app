namespace DoAn.HotelParking.Core.Application.DTOs.Parking;

public class LicensePlateLogDto
{
    public int Id { get; set; }
    public int ParkingSessionId { get; set; }
    public string? ImageUrl { get; set; }
    public string? DetectedPlate { get; set; }
    public decimal Confidence { get; set; }
    public DateTime CreatedAt { get; set; }
}