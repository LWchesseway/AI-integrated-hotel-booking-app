namespace DoAn.HotelParking.Core.Domain.Entities.Parking;

public class LicensePlateLog
{
    public int Id { get; set; }
    public int ParkingSessionId { get; set; }
    public string? ImageUrl { get; set; }
    public string? DetectedPlate { get; set; }
    public decimal Confidence { get; set; }
    public DateTime CreatedAt { get; set; }

    public ParkingSession ParkingSession { get; set; } = null!;
}